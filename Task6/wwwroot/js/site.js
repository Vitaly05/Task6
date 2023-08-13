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
        $('.messages').stop()
        addTagsHints(messages)
    })

    hubConnection.on('Tags', function(tags) {
        tagInput.setAutocompleteList(tags)
        messageTags.setAutocompleteList(tags)
    })
    
    chat.options.onSend = function(msg) {
        hubConnection.invoke('Send', msg.text, messageTags.tags())
    }

    tagsDisplay.options.onTagAdd = function(_, val) {
        $('.tags-display-panel').scrollTop($('.tags-display-panel').children().height())
        hubConnection.invoke('AddTag', val)
        hubConnection.invoke('GetMessages', tagsDisplay.tags())
    }

    tagsDisplay.options.onTagRemove = function() {
        hubConnection.invoke('GetMessages', tagsDisplay.tags())
    }

    tagsDisplay.options.onBeforeTagRemove = function(_, val,) {
        hubConnection.invoke('RemoveTag', val)
        return true
    }

    tagInput.options.onBeforeTagAdd = function(val) {
        if (tagsDisplay.tags().includes(val)) return false
        tagsDisplay._addTag(val)
        return false
    }
    messageTags.options.onBeforeTagAdd = function(val) {
        if (messageTags.tags().includes(val)) return false
        return true
    }

    $('.tags-display .input-clear-button').click(function() {
        hubConnection.invoke('GetMessages', [])
    })

    $('#show-tags-button').click(function() {
        if ($('#tags-panel').css('display') === 'none') {
            $(this).text("Hide your tags")
        } else {
            $(this).text("Show your tags")
        }
        $('#tags-panel').toggle()
    })

    checkWindowWidth()
    $(window).resize(checkWindowWidth)
    
    await hubConnection.start()

    hubConnection.invoke('GetMessages', tagsDisplay.tags())
    hubConnection.invoke('GetTags')
})

function checkWindowWidth() {
    console.log($(window).width())
    if ($(window).width() < 768) {
        $('#tags-panel').hide()
    } else {
        $('#tags-panel').show()
    }
}

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
        id: message.id,
        text: message.data,
        time: 0,
        position: 'left'
    }
}

function addTagsHints(messages) {
    messages.forEach(message => {
        if (message.tags.length > 0) {
            $(`.message[id="${message.id}"] .message-text `).append(`
                <span class="badge bg-green fg-white" data-role="hint" data-hint-position="right" 
                    data-cls-hint="bg-cyan fg-white drop-shadow" 
                    data-hint-text="${message.tags.map(t => `#${t.name}`).join(' ')}">
                    ${message.tags.length}
                </span>`)
        }
    })
}