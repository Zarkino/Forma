import * as monaco from '../../node_modules/monaco-editor/esm/vs/editor/editor.api';

type Monaco = typeof monaco;

let keywords = ['lemma', 'proof', 'from', 'by', 'next', 'have']
let components = ['assume', 'show']
let connectives = ['and']
let values = ['T', 'F', '⊤', '⊥']

const FORMAT: monaco.languages.IMonarchLanguage = {
    keywords,
    components,
    connectives,
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

const THEME_LIGHT: monaco.editor.IStandaloneThemeData = {
    base: 'vs',
    inherit: true,
    rules: [
        { token: 'keyword', foreground: '#006699', fontStyle: 'bold'},
        { token: 'component', foreground: '#0099FF', fontStyle: 'bold'},
        { token: 'connective', foreground: '#009966', fontStyle: 'bold'},
        { token: 'value', foreground: '#000000', fontStyle: 'bold'},
        { token: 'comment', foreground: '#2eb82e' },
        { token: 'neutral', foreground: '#000000'},
        { token: 'illegal', foreground: '#FF0000', fontStyle: 'bold'}
    ],
    colors: {}
}

const THEME_DARK: monaco.editor.IStandaloneThemeData = {
    base: 'vs-dark',
    inherit: true,
    rules: [
        { token: 'keyword', foreground: '#0077FF', fontStyle: 'bold'},
        { token: 'component', foreground: '#00CCFF', fontStyle: 'bold'},
        { token: 'connective', foreground: '#00CC88', fontStyle: 'bold'},
        { token: 'value', foreground: '#CCCCCC', fontStyle: 'bold'},
        { token: 'comment', foreground: '#2eb82e' },
        { token: 'neutral', foreground: '#CCCCCC'},
        { token: 'illegal', foreground: '#FF0000', fontStyle: 'bold'}
    ],
    colors: {}
}

export function beforeMount(monaco: Monaco) {
    monaco.languages.register({ id: "logi-lang" });
    monaco.languages.setMonarchTokensProvider("logi-lang", FORMAT);
    monaco.editor.defineTheme("logi-theme-light", THEME_LIGHT);
    monaco.editor.defineTheme("logi-theme-dark", THEME_DARK);
}