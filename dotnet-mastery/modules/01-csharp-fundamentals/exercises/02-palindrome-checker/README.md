# Exercise 02: Palindrome Checker

## 🎯 Objective

Build a palindrome checker that validates whether words, phrases, and sentences are palindromes. This exercise practices:

- String manipulation
- Character comparison
- Algorithm design
- Control flow
- Method design
- Edge case handling

## 📋 Requirements

### Core Functionality

Create a `PalindromeChecker` class with the following methods:

1. **`IsPalindrome(string text)`**
   - Check if a single word is a palindrome
   - Case-insensitive
   - Return `bool`

2. **`IsPalindromePhrase(string text)`**
   - Check if phrase is palindrome (ignore spaces and punctuation)
   - Case-insensitive
   - Examples: "A man, a plan, a canal: Panama"
   - Return `bool`

3. **`GetLongestPalindrome(string text)`**
   - Find the longest palindromic substring
   - Return the palindrome string
   - If multiple same length, return first found

4. **`FindAllPalindromes(string text)`**
   - Find all palindromic words in a sentence
   - Minimum length: 3 characters
   - Return `List<string>`

5. **`IsPalindromeNumber(int number)`**
   - Check if a number is a palindrome
   - Example: 121, 12321
   - Return `bool`

### Validation

- Empty strings are NOT palindromes
- Single characters ARE palindromes
- Whitespace-only strings are NOT palindromes
- Handle null input gracefully

### Console Application

Create an interactive console app that:
1. Displays menu options
2. Accepts user input
3. Shows results with appropriate formatting
4. Handles invalid input
5. Allows multiple checks

## 📊 Example Interactions

```
=== Palindrome Checker ===
1. Check word
2. Check phrase
3. Find longest palindrome
4. Find all palindromes in text
5. Check number
6. Exit

Select option: 1
Enter word: racecar
✓ "racecar" IS a palindrome!

Select option: 2
Enter phrase: A man, a plan, a canal: Panama
✓ "A man, a plan, a canal: Panama" IS a palindrome!

Select option: 3
Enter text: abcbabcbabcba
Longest palindrome: "abcbabcba" (length: 9)

Select option: 4
Enter text: The kayak on the noon river saw radar
Palindromes found:
- kayak
- noon
- radar

Select option: 5
Enter number: 12321
✓ 12321 IS a palindrome!

Select option: 1
Enter word: hello
✗ "hello" is NOT a palindrome
```

## ✅ Acceptance Criteria

1. **All tests pass** (`dotnet test`)
2. **Handle edge cases:**
   - Empty strings
   - Single characters
   - Numbers with trailing zeros
   - Mixed case input
   - Special characters and spaces
3. **Use appropriate algorithms** (hint: two-pointer approach)
4. **Clean, readable code**
5. **XML documentation** on public methods

## 💡 Hints

### Two-Pointer Technique

```csharp
public bool IsPalindrome(string text)
{
    int left = 0;
    int right = text.Length - 1;

    while (left < right)
    {
        if (text[left] != text[right])
        {
            return false;
        }
        left++;
        right--;
    }

    return true;
}
```

### Character Filtering

```csharp
// Remove non-alphanumeric characters
string cleaned = new string(
    text.Where(char.IsLetterOrDigit).ToArray());
```

### Complexity Analysis

- **IsPalindrome:** O(n) time, O(1) space
- **IsPalindromePhrase:** O(n) time, O(n) space (for cleaned string)
- **GetLongestPalindrome:** O(n²) time (naive), O(n) time (Manacher's algorithm)

## 🎓 Learning Goals

- String manipulation techniques
- Two-pointer algorithm
- Character comparison
- Edge case handling
- Algorithm optimization
- Clean code principles

## ⏱️ Estimated Time

- Implementation: 45-60 minutes
- Testing: 15-20 minutes
- **Total: ~1-1.5 hours**

## 🚀 Bonus Challenges

1. **Implement Manacher's Algorithm** for O(n) longest palindrome
2. **Unicode Support**: Handle emoji palindromes
3. **Performance Benchmark**: Compare different algorithms
4. **Palindrome Generator**: Generate palindromes of given length
5. **File Processing**: Find palindromes in text files

---

**Ready?** Open `starter/` directory and begin implementation!
