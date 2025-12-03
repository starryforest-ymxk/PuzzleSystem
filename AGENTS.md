# AGENTS.md

## Project Overview
This repository contains a **Unity (C#)** project used to build a **puzzle framework** with a node-based visual logic editor (using the NodeCanvas plugin under `Assets/Plugins/ParadoxNotion`).

The goal of this project is to design a modular, event-driven puzzle system that allows:
- Node-based configuration of puzzle logic and triggers.
- Clear definition of trigger conditions and effects.
- Control of animation / NPC dialogue / environment events.
- Persistent progress saving.
- Support for branching and network-like puzzle graphs.

## Directory Layout
- `Assets/Overview` — Design documents for the puzzle framework.
- `Assets/Plugins/ParadoxNotion` — NodeCanvas plugin (core graph editor).
- `Assets/Scenes` — Unity test scenes for puzzle nodes.
- `Assets/Settings` — Project settings and serialization configurations.
- (Recommended) `Assets/Scripts` — Custom logic for puzzle runtime, event dispatchers, and progress saving.

## Guidelines for Codex
- **Do NOT attempt to install or launch Unity Editor.**
- Focus on editing and refactoring **C# gameplay/system scripts**.
- Never modify `.meta` files or delete Unity assets.
- Use standard Unity C# conventions:
  - Classes: PascalCase
  - Fields / variables: camelCase
  - Events and interfaces: prefix with `I` or `On`
- Avoid breaking serialized field names that appear in `NodeCanvas` graphs.

## Common Tasks
When editing code, focus on:
- Creating modular puzzle node classes (`BasePuzzleNode`, `ConditionNode`, `EffectNode`).
- Implementing interfaces for event-driven logic (`IPuzzleTrigger`, `IPuzzleEffect`).
- Extending `NodeCanvas` to support visual linking and state transitions.
- Designing data persistence (`PuzzleSaveData`).

## Build / Test
- The Unity Editor itself is **not available** in Codex Cloud.
- C# scripts should be **syntactically valid** and compilable in Unity 2022+.
- No PlayMode execution is required.

## Collaboration Notes
- Each PR should focus on a single feature or bug fix.
- Avoid large-scale file renames or moving Unity assets unless requested.
- Write descriptive commit messages in English or bilingual (EN/中文).
