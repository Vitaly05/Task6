$(async function() {
    const chat = Metro.getPlugin($('#chat'), 'chat')
    const tagInput = Metro.getPlugin($('#tags-input'), 'tag-input')
    const tagsDisplay = Metro.getPlugin($('#tags-display'), 'tag-input')
    const messageTags = Metro.getPlugin($('#message-tags-input'), 'tag-input')
    
    const hubConnection = new signalR.HubConnectionBuilder().withUrl('/chat').build()
    
    hubConnection.on('NewMessage', function() {
        hubConnection.invoke('GetMessages', getTags(tagsDisplay))
    })

    hubConnection.on('GetMessages', function(messages) {
        chat.clear()
        chat.addMessages(convertMessagesToObject(messages))
    })
    
    chat.options.onSend = function(msg) {
        hubConnection.invoke('Send', {
            data: msg.text,
            tags: getTags(messageTags)
        })
    }

    tagsDisplay.options.onTagAdd = function() {
        $('.tags-display-panel').scrollTop($('.tags-display-panel').children().height())
        hubConnection.invoke('GetMessages', getTags(tagsDisplay))
    }

    tagsDisplay.options.onTagRemove = function() {
        hubConnection.invoke('GetMessages', getTags(tagsDisplay))
    }

    tagInput.options.onBeforeTagAdd = function(val) {
        $('#tags-panel').scrollTop($('#tags-panel').children().height())
        tagsDisplay._addTag(val)
        return false
    }
    
    await hubConnection.start()

    hubConnection.invoke('GetMessages', getTags(tagsDisplay))
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