$(async function() {
    const chat = Metro.getPlugin($('#chat'), 'chat')
    const tagInput = Metro.getPlugin($('#tags-input'), 'tag-input')
    const tagsDisplay = Metro.getPlugin($('#tags-display'), 'tag-input')
    const messageTags = Metro.getPlugin($('#message-tags-input'), 'tag-input')
    
    const hubConnection = new signalR.HubConnectionBuilder().withUrl('/chat').build()
    
    hubConnection.on('NewMessage', function(message) {
        chat.add({
            text: message,
            time: 0,
            position: 'left'
        })
    })

    hubConnection.on('GetMessages', function(messages) {
        chat.clear()
        chat.addMessages(convertMessagesToObject(messages))
    })
    
    chat.options.onSend = function(msg) {
        hubConnection.invoke('Send', msg.text, messageTags.tags())
    }

    tagsDisplay.options.onTagAdd = function() {
        $('.tags-display-panel').scrollTop($('.tags-display-panel').children().height())
    }

    tagsDisplay.options.onTagRemove = function() {
        hubConnection.invoke('GetMessages', tagsDisplay.tags())
    }

    tagsDisplay.options.onBeforeTagRemove = function(_, val,) {
        hubConnection.invoke('RemoveTag', val)
        return true
    }

    tagInput.options.onBeforeTagAdd = function(val) {
        $('#tags-panel').scrollTop($('#tags-panel').children().height())
        if (tagsDisplay.tags().includes(val)) return false
        tagsDisplay._addTag(val)
        hubConnection.invoke('AddTag', val)
        return false
    }
    
    await hubConnection.start()

    hubConnection.invoke('GetMessages', tagsDisplay.tags())
    hubConnection.invoke('GetTags')
})

function getTags(source) {
    const tags = []
    source.tags().forEach(tag => tags.push({name: tag}))
    return tags
}

function convertMessagesToObject(messages) {
    const msg = []
    messages.forEach(m => msg.push(convertMessageToObject(m)))
    return msg
}

function convertMessageToObject(message) {
    return {
        text: message.data,
        time: 0,
        position: 'left'
    }
}