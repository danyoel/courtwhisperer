function keys(obj) {
    var karr = [];
    for (var k in obj) {
        karr.push(k);
    }
    return karr;
}


angular.module('starter.services', [])
// ultimately these would be retrieved remotely, not hardcoded into the app.
.factory('Forms', function () {
    var jurisdictions = {
        "DC" : [ "DC" ],
        "WA" : [ "King" ]
    };

    // map jurisdiction (state.county) to available forms
    var jurisForms = {
        'WA.King': ['WPF DR 01.0100', 'WPF DRPSCU 09.0200', 'WPF DRPSCU 09.0210', 'WPF DR 01.0200',
            'DOH 422-027', 'WPF DRPSCU 01.0250', 'WPF DRPSCU 01.0330', 'WPF DRPSCU 01.0310', 'WPF DR 01.0300'],
        'DC.DC': ['Revised-Complaint-for-Absolute-Divorce', 'complaint-for-absolute-divorce-consent-answer',
            'Revised-Complaint-for-Absolute-Divorce-contested-answer']
    };

    var forms = {
        'WPF DR 01.0100': {
            usable: true,
            name: 'Petition for Dissolution of Marriage',
            description: '(long description)',
            itemCount: 10,
            timeEstimate: [10, 20],
        },
        'WPF DRPSCU 09.0200': {
            usable: true,
            name: 'Confidential Information',
            description: '(long description)',
            itemCount: 10,
            timeEstimate: [10, 20],
        },
        'WPF DRPSCU 09.0210': {
            usable: false,
            name: 'Addendum to Confidential Information',
        },
        'WPF DR 01.0200': {
            usable: false,
            name: 'Summons',
        },
        'DOH 422-027': {
            usable: false,
            name: 'Certificate of Dissolution, Declaration of Invalidity of Marriage, or Legal Separation',
        },
        'WPF DRPSCU 01.0250': {
            usable: false,
            name: 'Return of Service',
        },
        'WPF DRPSCU 01.0330': {
            usable: false,
            name: 'Joinder',
        },
        'WPF DRPSCU 01.0310': {
            usable: false,
            name: 'Acceptance of Service',
        },
        'WPF DR 01.0300': {
            usable: false,
            name: 'Response to Petition (Marriage)',
        },
        // DC forms
        'Revised-Complaint-for-Absolute-Divorce': {
            usable: false,
            name: 'Complaint for Absolute Divorce',
        },
        'complaint-for-absolute-divorce-consent-answer': {
            usable: false,
            name: 'Consent Answer to Complaint for Absolute Divorce',
        },
        'Revised-Complaint-for-Absolute-Divorce-contested-answer': {
            usable: false,
            name: 'Contested Answer to Complaint for Absolute Divorce',
        },
    };


    // default values that will be merged with each field
    var fieldDefaults = {
        required: true,
        enabled: true,
        output: true
    };


    var fields = {
        'WPF DR 01.0100': [
            {
                id: 'petitioner-name',
                bookmarks: [],
                name: 'Your full name',
                description: 'Your name as it appears on your driver\'s licence or ID card',
                inputType: 'text',
            },
            {
                id: 'petitioner-birth-date',
                bookmarks: [],
                name: 'Your date of birth',
                description: null,
                inputType: 'date',
            },
            {
                id: 'petitioner-location',
                required: true,
                bookmarks: [],
                name: 'Current location',
                description: 'Where do you live today? (County and state only)',
                inputType: 'text',
            },
            {
                id: 'respondent-name',
                required: true,
                bookmarks: [],
                name: 'Your spouse\'s name',
                description: 'Your spouse\'s name as it appears on his or her driver\'s licence or ID card',
                inputType: 'text',
            },
            {
                id: 'respondent-birth-date',
                bookmarks: [],
                name: 'Spouse\'s birth date',
                description: null,
                inputType: 'date',
            },
            {
                id: 'respondent-location',
                bookmarks: [],
                name: 'Current location',
                description: 'Where does your spouse live today? (County and state only)',
                inputType: 'text',
            },
            {
                id: 'marriage-location',
                bookmarks: [],
                name: 'Place of marriage',
                description: 'Where did you get married? (County and state only)',
                inputType: 'text',
            },
            {
                id: 'marriage-date',
                bookmarks: [],
                name: 'Date of the marriage',
                description: 'When did you get married?',
                inputType: 'date',
            },
            {
                id: 'children',
                output: false, // response will be excluded from data posted to server
                name: 'Children',
                description: 'Do you or your spouse have any dependent children?',
                inputType: 'boolean',
                onChange: function (form, value) {
                    form.enable(value, ['dual-dependents', 'petitioner-dependents', 'respondent-dependents']); 
                }
            },
            {
                id: 'dual-dependents',
                output: false,
                name: 'Children of both spouses',
                description: 'For how many children are you and your spouse BOTH the legal (biological or adoptive) parents?',
                inputType: 'select',
                values: [1, 2, 3, 4, 5, 6],
                onChange: function (form, value) {
                    for (var i = 0; i < parseInt(value) ; i++) {
                        form.enable('dual-children-' + i + '-name', true);
                        form.enable('dual-children-' + i + '-dob', true);
                    }
                }
            },
            {
                id: 'petitioner-dependents',
                output: false,
                name: 'Children of petitioner',
                description: 'For how many children are ONLY you and NOT your spouse the legal (biological or adoptive) parent?',
                inputType: 'select',
                values: ['None', '1', '2'],
                onChange: function (form, value) {
                    if (value !== 'None') value = 0; else value = parseInt(value);
                    for (var i = 0; i < 3; i++) {
                        form.enable('petitioner-children-' + i + '-name', i < value);
                        form.enable('petitioner-children-' + i + '-dob', i < value);
                    }
                }
            },
            {
                id: 'respondent-dependents',
                output: false,
                name: 'Children of petitioner',
                description: 'For how many children are ONLY your spouse and NOT yourself the legal (biological or adoptive) parent?',
                inputType: 'select',
                values: ['None', '1', '2'],
                onChange: function (form, value) {
                    if (value !== 'None') value = 0; else value = parseInt(value);
                    for (var i = 0; i < 3; i++) {
                        form.enable('respondent-children-' + i + '-name', i < value);
                        form.enable('respondent-children-' + i + '-dob', i < value);
                    }
                }
            },
            {
                id: 'dual-children-0-name',
                bookmarks: [],
                name: 'Child\'s name',
                description: 'First and last name of the child',
                inputType: 'text',
                enabled: false,
            },
            {
                id: 'dual-children-0-dob',
                bookmarks: [],
                name: 'Child\'s birthday',
                description: 'This child\'s date of birth', // ideally we'd template with the name of the child :)
                inputType: 'date',
                enabled: false,
            },
            {
                id: 'dual-children-1-name',
                bookmarks: [],
                name: 'Child\'s name',
                description: 'First and last name of the child',
                inputType: 'text',
                enabled: false,
            },
            {
                id: 'dual-children-1-dob',
                bookmarks: [],
                name: 'Child\'s birthday',
                description: 'This child\'s date of birth', // ideally we'd template with the name of the child :)
                inputType: 'date',
                enabled: false,
            },
            {
                id: 'dual-children-2-name',
                bookmarks: [],
                name: 'Child\'s name',
                description: 'First and last name of the child',
                inputType: 'text',
                enabled: false,
            },
            {
                id: 'dual-children-2-dob',
                bookmarks: [],
                name: 'Child\'s birthday',
                description: 'This child\'s date of birth', // ideally we'd template with the name of the child :)
                inputType: 'date',
                enabled: false,
            },
            {
                id: 'dual-children-3-name',
                bookmarks: [],
                name: 'Child\'s name',
                description: 'First and last name of the child',
                inputType: 'text',
                enabled: false,
            },
            {
                id: 'dual-children-3-dob',
                bookmarks: [],
                name: 'Child\'s birthday',
                description: 'This child\'s date of birth', // ideally we'd template with the name of the child :)
                inputType: 'date',
                enabled: false,
            },
            {
                id: 'dual-children-4-name',
                bookmarks: [],
                name: 'Child\'s name',
                description: 'First and last name of the child',
                inputType: 'text',
                enabled: false,
            },
            {
                id: 'dual-children-4-dob',
                bookmarks: [],
                name: 'Child\'s birthday',
                description: 'This child\'s date of birth', // ideally we'd template with the name of the child :)
                inputType: 'date',
                enabled: false,
            },
            {
                id: 'dual-children-5-name',
                bookmarks: [],
                name: 'Child\'s name',
                description: 'First and last name of the child',
                inputType: 'text',
                enabled: false,
            },
            {
                id: 'dual-children-5-dob',
                bookmarks: [],
                name: 'Child\'s birthday',
                description: 'This child\'s date of birth', // ideally we'd template with the name of the child :)
                inputType: 'date',
                enabled: false,
            },
            // petitioner only
            {
                id: 'petitioner-children-0-name',
                bookmarks: [],
                name: 'Child\'s name',
                description: 'First and last name of the child',
                inputType: 'text',
                enabled: false,
            },
            {
                id: 'petitioner-children-0-dob',
                bookmarks: [],
                name: 'Child\'s birthday',
                description: 'This child\'s date of birth', // ideally we'd template with the name of the child :)
                inputType: 'date',
                enabled: false,
            },
            {
                id: 'petitioner-children-1-name',
                bookmarks: [],
                name: 'Child\'s name',
                description: 'First and last name of the child',
                inputType: 'text',
                enabled: false,
            },
            {
                id: 'petitioner-children-1-dob',
                bookmarks: [],
                name: 'Child\'s birthday',
                description: 'This child\'s date of birth', // ideally we'd template with the name of the child :)
                inputType: 'date',
                enabled: false,
            },
            // respondent only
            {
                id: 'respondent-children-0-name',
                bookmarks: [],
                name: 'Child\'s name',
                description: 'First and last name of the child',
                inputType: 'text',
                enabled: false,
            },
            {
                id: 'respondent-children-0-dob',
                bookmarks: [],
                name: 'Child\'s birthday',
                description: 'This child\'s date of birth', // ideally we'd template with the name of the child :)
                inputType: 'date',
                enabled: false,
            },
            {
                id: 'respondent-children-1-name',
                bookmarks: [],
                name: 'Child\'s name',
                description: 'First and last name of the child',
                inputType: 'text',
                enabled: false,
            },
            {
                id: 'respondent-children-1-dob',
                bookmarks: [],
                name: 'Child\'s birthday',
                description: 'This child\'s date of birth', // ideally we'd template with the name of the child :)
                inputType: 'date',
                enabled: false,
            },
        ]
    };

    return {
        states: function () { return keys(jurisdictions); }, // Android lacks .keys() function
        counties: function (state) { return jurisdictions[state]; }, 
        byJurisdiction: function (state, county) {
            return jurisForms[state + '.' + county].reduce(function (accum, cur) {
                accum[cur] = forms[cur];
                return accum;
            }, {});
        },
        get: function (id) { return forms[id]; },
        fields: function (id) { return fields[id]; }
    };
})

;
