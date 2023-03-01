
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
            
            //TODO: The negative lookbehind is not working as intended in monaco/monarch, we need a fix or alternative
            // current workaround is to highlight functions and variables with the same color
            
            [/[a-z][\w$]*(?<=[^A-Z!?][a-z]*)(?=\s*\()/, { 
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

            [/[a-zA-Z][\w$]*(?=\s*\{)/, 'macro'], //Would (maybe?) be better to match behind with the def
            
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
        { token: 'constant', foreground: '#6B7785'},
        { token: 'predicate', foreground: '#570861'},
        { token: 'function', foreground: '#ff6b35'},
        { token: 'macro', foreground: '#ff4000'},
        { token: 'value', foreground: '#000000'},
        { token: 'illegal', foreground: '#FF0000', fontStyle: 'bold'}
    ],
    colors: {}
}

export const ASP_THEME_DARK = { //TODO: transfer tokens to dark mode and fix colors
    base: 'vs-dark',
    inherit: true,
    rules: [
        { token: 'keyword', foreground: '#ff3300', fontStyle: 'bold'},
        { token: 'operator', foreground: '#3366ee' },
        { token: 'comment', foreground: '#1ea81e' },
        { token: 'variable', foreground: '#ff6b35'},
        { token: 'constant', foreground: '#B7C9E2'},
        { token: 'predicate', foreground: '#A020F0'},
        { token: 'function', foreground: '#ff6b35'},
        { token: 'macro', foreground: '#ff4000'},
        { token: 'value', foreground: '#FFFFFF'},
        { token: 'illegal', foreground: '#FF0000', fontStyle: 'bold'}
    ],
    colors: {}
}
export const REGEX_TEST = RegExp(/[a-z][\w$]*(?<![A-Z!?][a-z]*)(?=\s*\()/, "g");