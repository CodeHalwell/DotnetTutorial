# Contributing to .NET Mastery

Thank you for your interest in contributing to the .NET Mastery tutorial! This document provides guidelines and instructions for contributors.

## 🎯 How to Contribute

### Types of Contributions

We welcome:

- **Bug fixes**: Typos, code errors, broken links
- **Content improvements**: Better explanations, additional examples
- **New exercises**: Well-designed practice problems with solutions
- **Code samples**: Real-world examples and projects
- **Documentation**: Clarifications, additional resources
- **Translations**: Help make this accessible to non-English speakers

### Areas We Need Help

- [ ] Additional exercises for each module
- [ ] More real-world project examples
- [ ] Video tutorials
- [ ] Interactive coding challenges
- [ ] Translations (Spanish, French, German, Chinese, Japanese, etc.)
- [ ] Accessibility improvements
- [ ] Mobile-friendly formatting

## 📋 Contribution Process

### 1. Fork and Clone

```bash
# Fork the repository on GitHub

# Clone your fork
git clone https://github.com/YOUR_USERNAME/dotnet-mastery.git
cd dotnet-mastery

# Add upstream remote
git remote add upstream https://github.com/ORIGINAL_OWNER/dotnet-mastery.git
```

### 2. Create a Branch

```bash
# Create a feature branch
git checkout -b feature/your-feature-name

# Or for bug fixes
git checkout -b fix/issue-description
```

### 3. Make Changes

Follow our content guidelines (see below).

### 4. Test Your Changes

- Ensure all code compiles
- Run all tests (`dotnet test`)
- Verify markdown renders correctly
- Check all links work

### 5. Commit

```bash
# Stage your changes
git add .

# Commit with descriptive message
git commit -m "Add: Temperature converter exercise solution"
```

**Commit message format:**
- `Add: New feature or content`
- `Fix: Bug fix or correction`
- `Update: Improvements to existing content`
- `Docs: Documentation changes`
- `Refactor: Code restructuring`

### 6. Push and Create Pull Request

```bash
# Push to your fork
git push origin feature/your-feature-name

# Create PR on GitHub
```

## 📝 Content Guidelines

### Code Quality

All code must:

1. **Follow C# conventions**
   - PascalCase for public members
   - camelCase for private fields
   - Use `var` appropriately
   - Include XML documentation comments

2. **Be production-quality**
   - Proper error handling
   - Input validation
   - Clear variable names
   - No magic numbers

3. **Include explanations**
   - Why this approach?
   - What are alternatives?
   - Common pitfalls?
   - Performance implications?

4. **Compile and run**
   - Test all code samples
   - Include complete, runnable examples
   - Provide project files when appropriate

### Writing Style

- **Clear and concise**: Avoid jargon, explain concepts simply
- **Active voice**: "The method returns" not "The method is returned"
- **Examples**: Show, don't just tell
- **Progressive**: Build on previous knowledge
- **Inclusive**: Use gender-neutral language

### Markdown Formatting

```markdown
# Use H1 for module/lesson title
## Use H2 for major sections
### Use H3 for subsections

**Bold** for emphasis
*Italic* for terms
`code` for inline code

\`\`\`csharp
// Use fenced code blocks with language
public void Example() { }
\`\`\`

> Use blockquotes for important notes

- Use bullet points for lists
1. Use numbers for steps
```

### Exercise Guidelines

Every exercise should include:

1. **README.md**
   - Clear objective
   - Requirements
   - Acceptance criteria
   - Example interactions
   - Time estimate

2. **Starter code**
   - Project files (.csproj)
   - Skeleton classes
   - TODO comments
   - Unit test template

3. **Solution**
   - Complete implementation
   - Detailed explanation
   - Alternative approaches
   - Common mistakes to avoid

4. **Tests**
   - Comprehensive unit tests
   - Edge cases covered
   - Clear test names

## 🔍 Review Process

### What We Look For

✅ **Technical accuracy**: Code must be correct and follow best practices
✅ **Clarity**: Explanations must be clear and understandable
✅ **Completeness**: No incomplete or TODO sections
✅ **Consistency**: Follow existing format and style
✅ **Value**: Adds meaningful content

### Feedback

- All PRs are reviewed by maintainers
- We may request changes
- Be patient - reviews may take a few days
- Engage constructively with feedback

## 🐛 Reporting Issues

### Before Submitting

1. Check if issue already exists
2. Verify it's actually a bug
3. Gather relevant information

### Issue Template

```markdown
**Description**
Clear description of the issue

**Location**
Module/lesson/exercise/file path

**Expected Behavior**
What should happen

**Actual Behavior**
What actually happens

**Steps to Reproduce**
1. Step one
2. Step two
3. ...

**Environment**
- OS: [e.g., Windows 11]
- .NET version: [e.g., 10.0.0]
- IDE: [e.g., VS 2025]

**Additional Context**
Any other relevant information
```

## 💡 Suggesting Enhancements

We love new ideas! When suggesting enhancements:

1. **Check existing issues** first
2. **Describe the problem** it solves
3. **Propose a solution** if you have one
4. **Consider alternatives**
5. **Think about impact** on existing content

## 🏆 Recognition

Contributors are recognized in:

- [CONTRIBUTORS.md](CONTRIBUTORS.md) file
- Release notes
- Project README

Significant contributions may earn you:
- Collaborator status
- GitHub profile badge
- Listed as co-author

## 📚 Resources

### .NET Resources
- [Microsoft .NET Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [C# Language Specification](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/)
- [.NET Foundation](https://dotnetfoundation.org/)

### Writing Resources
- [Google Developer Documentation Style Guide](https://developers.google.com/style)
- [Microsoft Writing Style Guide](https://learn.microsoft.com/en-us/style-guide/welcome/)

### Markdown
- [GitHub Flavored Markdown](https://github.github.com/gfm/)
- [Markdown Guide](https://www.markdownguide.org/)

## 📞 Contact

- **GitHub Issues**: For bugs and feature requests
- **Discussions**: For questions and general discussion
- **Email**: maintainer@example.com

## ⚖️ License

By contributing, you agree that your contributions will be licensed under the MIT License.

## 🙏 Thank You!

Your contributions help thousands of developers learn .NET. We appreciate your time and effort!

---

**Questions?** Feel free to ask in [GitHub Discussions](https://github.com/YOUR_REPO/discussions)!
