export default [
  {
    files: ["**/*.js", "**/*.mjs", "**/*.cjs", "**/*.ts"],
    languageOptions: {
      ecmaVersion: "latest",
      sourceType: "module",
    },
    rules: {
      semi: "error",
      "prefer-const": "error",
    },
  },
];
