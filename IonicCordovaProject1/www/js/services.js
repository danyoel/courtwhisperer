angular.module('starter.services', [])

.factory('Forms', function () {
    var forms = {
        'wa-disso.2015' : { name: 'Dissolution of Marriage', description: '(long description)' }
    };

    var fields = {
        'wa-disso.2015' : [
                {
                    id: 'full-name',
                    bookmarks: ['name01', 'name02'],
                    name: 'Your full name',
                    description: 'Your name as it appears on your driver\'s licence or ID card',
                    inputType: 'text',
                },
                {
                    id: 'marriage-date',
                    bookmarks: ['marriage-date'],
                    name: 'Date of the marriage',
                    description: 'Your name as it appears on your driver\'s licence or ID card',
                    inputType: 'date',
                }
        ]
    };

    return {
        all: function () { return forms; },
        get: function (id) { return forms[id]; },
        fields: function (id) { return fields[id]; }
    };
})

;
