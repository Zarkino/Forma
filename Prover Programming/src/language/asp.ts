export const ASP_FORMAT = {
    tokenizer: {
        root: [
            [/\[error.*/, "custom-error"],
            [/\[notice.*/, "custom-notice"],
            [/\[info.*/, "custom-info"],
            [/\[[a-zA-Z 0-9:]+\]/, "custom-date"],
        ],
    },
};

export const ASP_THEME = {
    base: 'vs',
    inherit: true,
    rules: [
        { token: "comment", foreground: "87a1c4" },
        { token: "number", foreground: "256fd1"},
        { token: "identifier", foreground: "586677" },
        { token: "keyword", foreground: "29ff00"},
        { token: "string", foreground: "7c71f2"}
    ],
    colors: {
        "editorCursor.foreground": "#586677",
        "editor.lineHighlightBackground": "#f9fcff",
    }
}