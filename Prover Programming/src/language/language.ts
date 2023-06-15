import * as monaco from '../../node_modules/monaco-editor/esm/vs/editor/editor.api';

type Monaco = typeof monaco;

let keyword1 = ['lemma', 'proof', 'from', 'by', 'next', 'have']
let keyword2 = ['and']
let keyword3 = ['assume', 'show']
let values = ['T', 'F', '⊤', '⊥']

import styles from '../styles/language.module.scss';

const FORMAT: monaco.languages.IMonarchLanguage = {
    keyword1,
    keyword2,
    keyword3,
    values,
    symbols:  /[=><!~?:&|+\-*\/\^%]+/,
    tokenizer: {
        root: [
            [/[a-zA-Z][\w$:]*/, {
                cases: {
                    '@keyword1': 'keyword1',
                    '@keyword2': 'keyword2',
                    '@keyword3': 'keyword3',
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
        { token: 'keyword1', foreground: styles.light_keyword1, fontStyle: 'bold' },
        { token: 'keyword2', foreground: styles.light_keyword2, fontStyle: 'bold' },
        { token: 'keyword3', foreground: styles.light_keyword3, fontStyle: 'bold' },
        { token: 'value', foreground: styles.light_value, fontStyle: 'bold' },
        { token: 'comment', foreground: styles.light_comment },
        { token: 'neutral', foreground: styles.light_neutral },
        { token: 'illegal', foreground: styles.light_illegal, fontStyle: 'bold' }
    ],
    colors: {}
}

const THEME_DARK: monaco.editor.IStandaloneThemeData = {
    base: 'vs-dark',
    inherit: true,
    rules: [
        { token: 'keyword1', foreground: styles.dark_keyword1, fontStyle: 'bold' },
        { token: 'keyword2', foreground: styles.dark_keyword2, fontStyle: 'bold' },
        { token: 'keyword3', foreground: styles.dark_keyword3, fontStyle: 'bold' },
        { token: 'value', foreground: styles.dark_value, fontStyle: 'bold' },
        { token: 'comment', foreground: styles.dark_comment },
        { token: 'neutral', foreground: styles.dark_neutral },
        { token: 'illegal', foreground: styles.dark_illegal, fontStyle: 'bold' }
    ],
    colors: {}
}

export function beforeMount(monaco: Monaco) {
    monaco.languages.register({ id: "logi-lang" });
    monaco.languages.setMonarchTokensProvider("logi-lang", FORMAT);
    monaco.editor.defineTheme("logi-theme-light", THEME_LIGHT);
    monaco.editor.defineTheme("logi-theme-dark", THEME_DARK);
}