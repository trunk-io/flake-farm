# AGENTS Instructions

## Formatting and Linting

- Run `trunk check -y --filter=-trufflehog` after modifying code to automatically format and fix linter issues.
- Review and verify all changes made by `trunk check` are correct before committing.
- If only formatting is needed, run `trunk fmt`.
- If any linter requires network access, add it to the negative filter list when
  running Trunk (for example, `trunk check -y --filter=-trufflehog`).
