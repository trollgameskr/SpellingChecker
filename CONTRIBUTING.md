# Contributing to SpellingChecker

Thank you for your interest in contributing to SpellingChecker! This document provides guidelines and instructions for contributing.

## Code of Conduct

Be respectful and inclusive. We welcome contributions from everyone.

## How Can I Contribute?

### Reporting Bugs

Before creating bug reports, please check existing issues to avoid duplicates. When creating a bug report, include:

- **Clear title and description**
- **Steps to reproduce** the issue
- **Expected behavior** vs actual behavior
- **Screenshots** if applicable
- **System information**: Windows version, .NET version
- **Application version**: Check About or version number

### Suggesting Enhancements

Enhancement suggestions are welcome! Please include:

- **Clear use case** - what problem does it solve?
- **Proposed solution** - how would it work?
- **Alternatives considered**
- **Additional context** - mockups, examples, etc.

### Pull Requests

1. **Fork** the repository
2. **Create a branch** from `develop`:
   ```bash
   git checkout -b feature/your-feature-name
   ```
3. **Make your changes**:
   - Follow the coding style (see below)
   - Add tests if applicable
   - Update documentation as needed
4. **Test your changes** thoroughly
5. **Commit** with clear messages:
   ```bash
   git commit -m "Add feature: description of feature"
   ```
6. **Push** to your fork:
   ```bash
   git push origin feature/your-feature-name
   ```
7. **Open a Pull Request** against the `develop` branch

## Development Setup

See [BUILD.md](BUILD.md) for detailed development environment setup.

Quick start:
```bash
git clone https://github.com/shinepcs/SpellingChecker.git
cd SpellingChecker
dotnet restore
dotnet build
```

## Coding Style

### C# Guidelines

- Follow [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use meaningful variable and method names
- Add XML documentation comments for public methods
- Keep methods focused and small (single responsibility)

### Example:

```csharp
/// <summary>
/// Corrects spelling and grammar in the provided text
/// </summary>
/// <param name="text">The text to correct</param>
/// <returns>Correction result with original and corrected text</returns>
public async Task<CorrectionResult> CorrectSpellingAsync(string text)
{
    // Implementation
}
```

### XAML Guidelines

- Use consistent indentation (4 spaces)
- Group related properties together
- Use meaningful names for x:Name attributes
- Keep XAML clean and readable

### Naming Conventions

- **Classes**: PascalCase (e.g., `AIService`, `ResultPopupWindow`)
- **Methods**: PascalCase (e.g., `CorrectSpellingAsync`)
- **Private fields**: _camelCase (e.g., `_apiService`)
- **Properties**: PascalCase (e.g., `ApiKey`)
- **Local variables**: camelCase (e.g., `selectedText`)

## Project Structure

```
SpellingChecker/
├── Models/           # Data models and DTOs
├── Services/         # Business logic and external integrations
├── Views/            # UI windows and controls
├── Utils/            # Utility classes and helpers
└── Resources/        # Images, icons, strings
```

## Testing

### Manual Testing

Before submitting a PR, test:
1. Spelling correction with various text types
2. Translation (Korean ↔ English)
3. Hotkeys work globally
4. Settings save and load correctly
5. System tray integration
6. Copy and replace functions

### Automated Tests

If adding new features, consider adding unit tests:
```csharp
[TestMethod]
public async Task CorrectSpelling_ShouldFixErrors()
{
    // Arrange
    var service = new AIService(_settingsService);
    var text = "Ths is a tst";
    
    // Act
    var result = await service.CorrectSpellingAsync(text);
    
    // Assert
    Assert.IsNotNull(result);
    Assert.AreNotEqual(text, result.CorrectedText);
}
```

## Documentation

When adding new features:
- Update README.md if it affects user experience
- Update CONFIG.md for new settings
- Update BUILD.md for new build requirements
- Add XML comments to public APIs
- Update CHANGELOG.md

## Commit Messages

Use clear, descriptive commit messages:

**Format**:
```
<type>: <description>

[optional body]

[optional footer]
```

**Types**:
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code refactoring
- `test`: Adding or updating tests
- `chore`: Maintenance tasks

**Examples**:
```
feat: Add support for Japanese translation

Added Japanese language detection and translation support.
Updated language detection algorithm to handle Japanese characters.
```

```
fix: Hotkey registration fails on some systems

Fixed race condition in hotkey registration by adding retry logic.
Fixes #123
```

## Review Process

1. **Automated checks** must pass (build, tests)
2. **Code review** by maintainers
3. **Testing** verification
4. **Documentation** review
5. **Merge** to develop branch

## Feature Requests

Priority areas for contribution:

### High Priority
- Customizable hotkeys
- Additional language support
- Offline model integration
- Auto-update mechanism

### Medium Priority
- Change highlighting in results
- Usage statistics
- Dark mode theme
- Performance optimizations

### Low Priority
- Browser extension
- Context menu integration
- Custom dictionaries
- Voice input support

## Security

If you discover a security vulnerability:
1. **Do NOT** open a public issue
2. Email the maintainers privately
3. Include detailed reproduction steps
4. Allow time for a fix before public disclosure

## Questions?

- Open a discussion on GitHub
- Check existing issues and documentation
- Contact maintainers

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

## Recognition

Contributors will be recognized in:
- README.md contributors section
- Release notes
- Project documentation

Thank you for contributing to SpellingChecker! 🎉
