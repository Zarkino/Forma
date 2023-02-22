
let keywords = ['Def', 'DEF', 'def']
let operators = ['and', 'or', 'not', 'imply', 'bicon', 'forall', 'exists', 'thus', '&', '|', '!', '->', '<>', 'A', 'E', '||=']
let values = ['True', 'False', 'T', 'F']               

export const ASP_FORMAT = {
    keywords,
    operators,
    values,
    symbols:  /[=><!~?:&|+\-*\/\^%]+/,
    tokenizer: {
        root: [
            [/[A-Z][\w$]*\s*(?=\()/, 'predicate'],
            [/[a-z][\w$]*\s*(?=\()/, 'function'],
            
            [/(?!\w*\()[a-z][\w$]*/, {
                cases: {
                    '@keywords': 'keyword',
                    '@values': 'value',
                    '@default': 'variable', //TODO: don't match words starting with a large letter (or let variables be case-insensitive)
                }
            }],
            
            [/_[a-z][\w$]*/, 'constant'],
            [/\.[a-zA-Z][\w$]*/, 'macro'], //TODO: check if regex is good enough and fix the definition part

            { include: '@whitespace' },

            [/@symbols|A|E/, { //TODO: A E cases should be somewhat reformatted to be more specific
                cases: { 
                    '@operators': 'operator',
                    '@default'  : ''
                } 
            }]
            
        ],

        whitespace: [
            [/[ \t\r\n]+/, 'whitespace'],
            [/\/\*/,       'comment', '@comment' ],
            [/\/\/.*$/,    'comment'],
        ],

        comment: [
            [/[^\/*]+/, 'comment'],
            [/\*\//, 'comment', '@pop'],
            [/[\/*]/, 'comment']
        ]
        
    }
};

export const ASP_THEME_LIGHT = {
    base: 'vs',
    inherit: true,
    rules: [
        { token: 'keyword', foreground: '#ff3300', fontStyle: 'bold'},
        { token: 'operator', foreground: '#0033cc' },
        { token: 'comment', foreground: '#2eb82e' },
        { token: 'variable', foreground: '#FF6600'},
        { token: 'constant', foreground: '#cc3399'},
        { token: 'predicate', foreground: '#00ff00'},
        { token: 'function', foreground: '#6600ff'},
        { token: 'macro', foreground: '#cc9900'},
        { token: 'value', foreground: '#000000'}
    ],
    colors: {}
}

export const ASP_THEME_DARK = { //TODO: transfer tokens to dark mode and fix colors
    base: 'vs-dark',
    inherit: true,
    rules: [
        { token: 'keyword', foreground: '#FF6600', fontStyle: 'bold' },
        { token: 'comment', foreground: '#999999' },
        { token: 'string', foreground: '#009966'},
        { token: 'variable', foreground: '#006699'}
    ],
    colors: {}
}