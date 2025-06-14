# Time Riders Project Refactor Task List

## Summary of Comments, Criticisms, and Suggestions
- **SOLID and YAGNI Principles:** Only use interfaces when multiple implementations are likely. Avoid unnecessary abstractions.
- **Brace Style:** Opening braces must be on the same line as method signatures and control statements.
- **Beginner-Friendly:** Code and documentation should be clear and educational, suitable for new programmers (e.g., the user's teenage son).
- **Code Organization:** Scripts should be grouped by feature/domain. Keep directory structure flat (max 3-4 levels).
- **Documentation:** Use XML comments for all public classes and methods. Add regular comments to explain "why".
- **Testing:** Add unit and play mode tests for core gameplay logic.
- **Logging:** Implement a simple, categorized static logger. Use appropriate log levels.
- **ScriptableObjects:** Use for configuration data to separate data from behavior.
- **Public Methods at Top:** Place public methods at the top of each file, private helpers at the bottom.
- **Consistent Style Guides:** All instruction and style guide files must be consistent and up-to-date.
- **Refactor Setup Scripts:** Improve modularity and clarity of SceneSetup, QuickPlayerSetup, QuickUISetup.
- **Storytelling:** Clear design docs, README, comments, naming, logs, and tests to tell the code's story.
- **No Unnecessary Complexity:** Favor composition and component-based design over deep inheritance.
- **UI Setup:** QuickUISetup.cs belongs in the UI folder, as it is responsible for runtime UI creation/initialization.

---

## Refactor Checklist

- [ ] 1. Restore and update all style guide and instruction files for consistency
- [x] 2. Create and document improved folder structure for scripts
- [x] 3. Move scripts into new structure (Core, Player, UI, Environment, Setup, Config)
- [x] 4. Refactor `SceneSetup.cs` for clarity, XML docs, brace style, single responsibility
- [x] 5. Refactor `QuickPlayerSetup.cs` for clarity, XML docs, brace style, single responsibility
- [x] 6. Refactor `QuickUISetup.cs` for clarity, XML docs, brace style, single responsibility
- [x] 7. Add static logger utility (`GameLogger.cs`) in Core/
- [x] 8. Provide sample ScriptableObject config (`BikeConfig`) in Config/
- [ ] 9. Ensure all documentation files reflect current standards and decisions
- [ ] 10. Add or update README and design docs to tell the project story

_Note: README and documentation are now being updated to reflect the new architecture and standards._
- [ ] 11. Add or update unit and play mode tests for core gameplay logic
- [ ] 12. Review and update logging usage across scripts
- [ ] 13. Final review for beginner-friendliness and maintainability

---

**This checklist will be updated as progress is made.**
