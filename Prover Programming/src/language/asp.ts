
let keywords = ['Def', 'DEF', 'def']
let operators = ['and', 'or', 'not', 'imply', 'bicon', 'forall', 'exists', 'thus', '&', '|', '~', '->', '<->', '!', '?', '||=']
let values = ['True', 'False', 'true', 'false', 'T', 'F']               

export const ASP_FORMAT = {
    keywords,
    operators,
    values,
    symbols:  /[=><!~?:&|+\-*\/\^%]+/,
    tokenizer: {
        root: [
            [/[A-Z][\w$]*(?=\s*\()/, {
                cases: {
                    '@keywords': 'keyword',
                    '@values': 'value',
                    '@default': 'predicate',
                }
            }],

            [/[a-z][\w$]*(?<![A-Z!?][a-z]*)(?=\s*\()/, { //TODO: The negative lookbehind is not working as intended in monaco/monarch, we need a fix or alternative
                cases: {
                    '@keywords': 'keyword',
                    '@values': 'value',
                    '@operators': 'operator',
                    '@default': 'function',
                }
            }],

            [/[A-Z][\w$]*/, {
                cases: {
                    '@keywords': 'keyword',
                    '@values': 'value',
                    '@default': 'illegal',
                }
            }],
            
            [/[a-z][\w$]*/, {
                cases: {
                    '@keywords': 'keyword',
                    '@operators': 'operator',
                    '@values': 'value',
                    '@default': 'variable',
                }
            }],
            
            [/_[a-z][\w$]*/, 'constant'],
            [/\.[a-zA-Z][\w$]*/, 'macro'], //TODO: check if regex is good enough and fix the definition part

            { include: '@whitespace' },

            [/@symbols/, {
                cases: { 
                    '@operators': 'operator',
                    '@default'  : 'illegal'
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
        { token: 'variable', foreground: '#ff6b35'},
        { token: 'constant', foreground: '#cc3399'},
        { token: 'predicate', foreground: '#00ff00'},
        { token: 'function', foreground: '#ff6b35', fontStyle: 'bold'},
        { token: 'macro', foreground: '#cc9900'},
        { token: 'value', foreground: '#000000'},
        { token: 'illegal', foreground: '#FF0000', fontStyle: 'bold'}
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
export const REGEX_TEST = RegExp(/[a-z][\w$]*(?<![A-Z!?][a-z]*)(?=\s*\()/, "g");