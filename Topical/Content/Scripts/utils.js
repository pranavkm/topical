angular.module("utils", [])
    .factory("utils", function () {
        return {
            slugify: function(str) {
                // Based on http://jsfiddle.net/wraithmonster/AAwTT/
                // Separate camel-cased words with a space for later processing. 
                str = str.toLowerCase();

                // remove accents, swap ñ for n, etc
                var from = "àáäâèéëêìíïîòóöôùúüûñç·/_,:;",
                    to   = "aaaaeeeeiiiioooouuuunc------";
                for (var i = 0, l = from.length ; i < l ; i++) {
                    str = str.replace(from[i], to[i]);
                }

                return str.replace(/[^a-z0-9 -]/g, '') // remove invalid chars
                          .replace(/\s+/g, '-') // collapse whitespace and replace by -
                          .replace(/-+/g, '-') // collapse dashes
                          .replace(/^[\s|-]+|[\s|-]+$/g, '');
            },

            topicCommentsLink: function (topic) {
                if (!topic || !topic.title) {
                    return "";
                }

                var titleTokens = this.slugify(topic.title).split("-"),
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
