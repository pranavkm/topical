angular.module("utils", [])
    .factory("utils", function () {
        return {
            topicCommentsLink: function (topic) {
                if (!topic || !topic.title) {
                    return "";
                }

                var titleTokens = topic.title.split(' '),
                    length = 0,
                    allowedLength = 50;
                for (var i = 0; i < titleTokens.length; i++) {
                    if (i != 0) {
                        // One for the dash
                        length += 1;
                    }
                    length += titleTokens[i].length;
                    if (length > allowedLength) {
                        titleTokens = titleTokens.slice(0, i + 1);
                        break;
                    }
                }

                // Make the url look like {title-id}/{dasherized-truncated-title}/
                return "/topic/" + topic.id + "/" + titleTokens.join("-") + "/";
            }
        }
    });
