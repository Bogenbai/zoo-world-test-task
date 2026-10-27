# Git Workflow

Branching and commit rules for this repository.

## Branches

- **master** — always stable and working. Receives `dev` only when the work on it is finished and verified.
- **dev** — integration branch. Every working branch starts from `dev` and merges back into it.
- **feature/<description>** — one feature, e.g. `feature/object-pooling`.
- **bugfix/<description>** — a fix for something already merged into `dev`, e.g. `bugfix/pooled-object-reset`.

Every branch is named after the work it carries. Merge with `--no-ff` so the history shows where each feature started and ended.

## Commits

- **Never commit without the user's explicit approval of that specific commit.** Before committing, show what changed and propose the message, then wait. Approval of one commit does not carry over to the next, and an agreed commit split is not an approval to commit.
- One idea per commit. The message is a short English sentence about the behaviour change, not the files touched, often in the form `Feature: what changed` (`Pooling: reset instances when they return to the pool`).

## Merging and pushing

- AI assistants never merge branches into `dev` or `master`, never push, and never open or merge pull requests unless the user explicitly asks for that specific action.
- A short "yes" / "ok" to a question that mentions merging or pushing is not a request to merge or push.
