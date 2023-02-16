let keywords = ['class', 'new', 'string', 'number', 'boolean', 'private', 'public']

export const ASP_FORMAT = {
    keywords,
    tokenizer: {
        root: [
            [/@?[a-zA-Z][\w$]*/, {
                cases: {
                    '@keywords': 'keyword',
                    '@default': 'variable',
                }
            }],
            [/".*?"/, 'string'],
            [/\/\//, 'comment'],
        ]
    }
};

export const ASP_THEME_LIGHT = {
    base: 'vs',
    inherit: true,
    rules: [
        { token: 'keyword', foreground: '#FF6600', fontStyle: 'bold' },
        { token: 'comment', foreground: '#999999' },
        { token: 'string', foreground: '#009966'},
        { token: 'variable', foreground: '#006699'}
    ],
    colors: {}
}

export const ASP_THEME_DARK = {
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