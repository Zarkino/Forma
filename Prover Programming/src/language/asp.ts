import * as monaco from '../../node_modules/monaco-editor/esm/vs/editor/editor.api';

type Monaco = typeof monaco;

let keywords = ['lemma', 'proof', 'from', 'by', 'next', 'have']
let components = ['assume', 'show']
let connectives = ['and']
//let logicals = ['~', '¬', '&', '∧', '|', '∨', '-->', '⟶', '<-->', '⟷', '(', ')', 'none', 'this', '.', '-', '!!', '⋀', '==>', '⟹', '≡', '==', '{', '}']
let values = ['T', 'F', '⊤', '⊥']               

const ASP_FORMAT: monaco.languages.IMonarchLanguage = {
    keywords,
    components,
    connectives,
//    logicals,
    values,
    symbols:  /[=><!~?:&|+\-*\/\^%]+/,
    tokenizer: {
        root: [
            [/[a-zA-Z][\w$:]*/, {
                cases: {
                    '@keywords': 'keyword',
                    '@components': 'component',
                    '@connectives': 'connective',
                    '@values': 'value',
                    '@default': 'neutral',
                }
            }],

            [/[~¬&∧|∨\->⟶<⟷().!⋀=⟹≡{}]+/, 'neutral'],

            { include: '@whitespace' },

            [/@symbols/, {
                cases: {
                    '@default': 'illegal'
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

const ASP_THEME_LIGHT: monaco.editor.IStandaloneThemeData = {
    base: 'vs',
    inherit: true,
    rules: [
        { token: 'keyword', foreground: '#006699'},
        { token: 'component', foreground: '#0099FF'},
        { token: 'connective', foreground: '#009966' },
        { token: 'value', foreground: '#000000', fontStyle: 'bold'},
        { token: 'comment', foreground: '#2eb82e' },
        { token: 'neutral', foreground: '#000000'},
        { token: 'illegal', foreground: '#FF0000', fontStyle: 'bold'}
    ],
    colors: {}
}

const ASP_THEME_DARK: monaco.editor.IStandaloneThemeData = {
    base: 'vs-dark',
    inherit: true,
    rules: [
        { token: 'keyword', foreground: '#0077FF'},
        { token: 'component', foreground: '#00CCFF'},
        { token: 'connective', foreground: '#00CC88' },
        { token: 'value', foreground: '#CCCCCC', fontStyle: 'bold'},
        { token: 'comment', foreground: '#2eb82e' },
        { token: 'neutral', foreground: '#CCCCCC'},
        { token: 'illegal', foreground: '#FF0000', fontStyle: 'bold'}
    ],
    colors: {}
}

export function run(monaco: Monaco) {
    monaco.languages.register({ id: "asp-lang" });
    monaco.languages.setMonarchTokensProvider("asp-lang", ASP_FORMAT);
    monaco.editor.defineTheme("asp-theme-light", ASP_THEME_LIGHT);
    monaco.editor.defineTheme("asp-theme-dark", ASP_THEME_DARK);
}