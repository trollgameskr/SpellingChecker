# Examples and Best Practices

## Common Use Cases

### 1. Email Writing

#### Scenario: Writing a professional email in Korean, need English version

**Workflow**:
1. Write your email in Korean
2. Select the entire email body
3. Press `Ctrl+Shift+T` to translate to English
4. Press `Ctrl+Shift+Y` to polish the English version
5. Copy the result

**Example**:
```
Original (Korean):
안녕하세요, 
회의 일정을 다음주 월요일로 변경하고 싶습니다.
가능하신지 확인 부탁드립니다.

After Translation (Ctrl+Shift+T):
Hello,
I would like to change the meeting schedule to next Monday.
Please confirm if this is possible.

After Correction (Ctrl+Shift+Y):
Hello,
I would like to reschedule our meeting to next Monday.
Please let me know if this works for you.
```

### 2. Document Proofreading

#### Scenario: Long English document needs proofreading

**Best Practice**:
- Don't select the entire document at once
- Process paragraph by paragraph
- Review each correction before accepting

**Workflow**:
```
1. Select first paragraph
2. Ctrl+Shift+Y
3. Review correction → Replace if good
4. Select next paragraph
5. Repeat
```

**Why**: 
- Faster processing
- Easier to review changes
- Lower API costs
- More accurate results

### 3. Learning Korean/English

#### Scenario: Writing practice in target language

**Workflow for Korean learners**:
1. Write a sentence in Korean
2. Press `Ctrl+Shift+Y` to see corrections
3. Compare original vs corrected
4. Learn from the differences
5. Press `Ctrl+Shift+T` to see English translation

**Example learning session**:
```
My attempt:    "나는 학교에 가요 어제"
Correction:    "나는 어제 학교에 갔어요"
Translation:   "I went to school yesterday"

Lesson learned: 
- Past tense: 가요 → 갔어요
- Word order: time before location
```

### 4. Quick Message Translation

#### Scenario: Received Korean message, need to understand

**Workflow**:
1. Copy the Korean message
2. Paste into any text field
3. Select the text
4. Press `Ctrl+Shift+T`
5. Read the English translation

**Speed tip**: Keep Notepad open for quick paste-translate-read operations.

### 5. Social Media Posts

#### Scenario: Writing bilingual social media content

**Workflow**:
1. Write in your primary language
2. Translate with `Ctrl+Shift+T`
3. Correct both versions with `Ctrl+Shift+Y`
4. Post both versions

**Example**:
```
Korean post: 오늘 새로운 프로젝트를 시작했습니다! 🚀
English version (Ctrl+Shift+T): I started a new project today! 🚀
```

## Configuration Examples

### Example 1: Fast and Economical Setup

**Settings**:
```
API Endpoint: https://api.openai.com/v1
Model: gpt-3.5-turbo
Auto Start: Yes
```

**Use case**: High volume, cost-sensitive users

**Pros**:
- Fastest response time
- Lowest cost
- Good for simple corrections

**Cons**:
- Lower quality for complex texts
- May miss nuanced errors

### Example 2: Quality-Focused Setup

**Settings**:
```
API Endpoint: https://api.openai.com/v1
Model: gpt-4o
Auto Start: Yes
```

**Use case**: Professional documents, important emails

**Pros**:
- Highest quality
- Best for complex corrections
- Superior translation accuracy

**Cons**:
- Slower (3-5 seconds)
- Higher cost (~4x more than gpt-4o-mini)

### Example 3: Balanced Setup (Recommended)

**Settings**:
```
API Endpoint: https://api.openai.com/v1
Model: gpt-4o-mini
Auto Start: Yes
```

**Use case**: Daily use, general purpose

**Pros**:
- Good balance of speed and quality
- Reasonable cost
- Handles most corrections well

**Cons**:
- Slightly slower than gpt-3.5-turbo
- May not catch very subtle errors

### Example 4: Corporate/Azure Setup

**Settings**:
```
API Endpoint: https://your-company.openai.azure.com/
Model: gpt-4o-mini
Auto Start: Yes
```

**Use case**: Enterprise users with Azure OpenAI

**Requirements**:
- Azure OpenAI subscription
- Proper endpoint URL
- Azure API key (format may differ)

## Best Practices

### ✅ Do's

1. **Review Before Replacing**
   - Always read the correction/translation
   - Use "Copy" first if unsure
   - The AI isn't perfect

2. **Select Appropriate Text Amounts**
   - Paragraphs: ✅ Good
   - Full documents: ❌ Too much
   - Single words: ⚠️ Often unnecessary

3. **Use for Appropriate Content**
   - Emails: ✅ Excellent
   - Reports: ✅ Great
   - Messages: ✅ Perfect
   - Code: ⚠️ Comments only
   - Passwords: ❌ Never

4. **Keep API Key Secure**
   - Don't share with others
   - Regenerate if compromised
   - Don't commit to version control

5. **Monitor API Usage**
   - Check usage at https://platform.openai.com/usage
   - Set up billing alerts
   - Review monthly costs

6. **Update Regularly**
   - Check for new versions
   - Read release notes
   - Backup settings before updating

### ❌ Don'ts

1. **Don't Process Sensitive Data Without Consideration**
   - Personal information
   - Confidential business data
   - Passwords or credentials
   - Medical records
   - Legal documents (without approval)

2. **Don't Trust Blindly**
   - Always review AI suggestions
   - Especially for technical/specialized content
   - Context matters

3. **Don't Overuse on Same Text**
   - One correction pass is usually enough
   - Multiple passes can degrade quality
   - Wastes API credits

4. **Don't Process Already Correct Text**
   - AI might "fix" what isn't broken
   - Changes writing style unnecessarily
   - Costs money for no benefit

5. **Don't Use for Real-time Chat**
   - 1-3 second delay
   - Better for composed messages
   - Not suitable for live conversation

## Keyboard Efficiency Tips

### Workflow Optimization

**Standard workflow**:
```
1. Select text (mouse)
2. Ctrl+Shift+Y (keyboard)
3. Review popup (visual)
4. Click Replace (mouse)
```

**Faster workflow**:
```
1. Triple-click to select paragraph (mouse)
2. Ctrl+Shift+Y
3. Quick review
4. Click Replace
5. Triple-click next paragraph
6. Repeat
```

**Power user workflow**:
```
1. Keep one hand on mouse for selecting
2. Keep other hand on Ctrl+Shift+Y/T position
3. Use Tab to switch between popup buttons
4. Press Enter to activate selected button
```

## Multi-Language Documents

### Scenario: Document with Mixed Korean and English

**Challenge**: Text with both languages mixed together

**Best practice**:
1. Separate Korean and English sections
2. Process each language separately
3. Only use translation when needed, not correction

**Example**:
```
Original:
"This is a test. 이것은 테스트입니다. More English here."

Workflow:
1. Select "This is a test."
   → Ctrl+Shift+Y (correction only)

2. Select "이것은 테스트입니다."
   → Ctrl+Shift+Y (correction)
   → Or Ctrl+Shift+T (if need English version)

3. Select "More English here."
   → Ctrl+Shift+Y (correction only)
```

## Performance Optimization

### For Slow Internet

**Tips**:
1. Use gpt-3.5-turbo for faster response
2. Select smaller text chunks
3. Process during off-peak hours
4. Consider offline mode (future feature)

### For High Volume Use

**Optimization**:
1. Batch similar corrections together
2. Create templates for common phrases
3. Use simpler AI model for routine work
4. Reserve gpt-4o for critical content

### For Cost Reduction

**Strategies**:
1. **Model selection**:
   - Routine: gpt-3.5-turbo
   - Normal: gpt-4o-mini  
   - Important: gpt-4o

2. **Text selection**:
   - Select only parts that need correction
   - Don't reprocess same text
   - Skip already correct sections

3. **Monitoring**:
   - Set OpenAI billing alerts
   - Review usage weekly
   - Adjust model if over budget

## Integration with Other Tools

### Microsoft Word

**Best workflow**:
1. Write in Word using built-in spell check
2. For final polish, select paragraphs
3. Use SpellingChecker for AI-powered refinement
4. Especially useful for Korean text (Word's Korean support is limited)

### Email Clients

**Outlook**:
```
1. Compose email
2. Select body before sending
3. Ctrl+Shift+Y for final check
4. Send with confidence
```

**Gmail (browser)**:
```
1. Write email in compose window
2. Select text
3. Use SpellingChecker
4. Replace or paste
```

### Messaging Apps

**Slack, Teams, Discord**:
```
1. Type message in draft
2. Select before sending
3. Quick correction
4. Send polished message
```

**Tip**: For very short messages, correction might not be needed.

## Troubleshooting Common Issues

### Issue: Correction changes meaning

**Cause**: AI interprets context differently

**Solution**:
```
1. Select smaller, more specific text
2. Provide more context by selecting surrounding sentences
3. Review and reject if meaning changed
4. Consider using gpt-4o for better accuracy
```

### Issue: Translation sounds unnatural

**Cause**: Direct translation vs. localization

**Solution**:
```
1. Translate first (Ctrl+Shift+T)
2. Then correct the translation (Ctrl+Shift+Y)
3. This gives more natural target language
```

### Issue: Too many unnecessary changes

**Cause**: AI over-correcting

**Solution**:
```
1. Text might already be correct
2. Don't fix what isn't broken
3. Use selectively on problem areas only
```

## Success Metrics

### How to measure value

**Time savings**:
```
Before: 10 minutes to write and proofread email
After: 5 minutes to write, 10 seconds to correct
Savings: ~50% time reduction
```

**Quality improvement**:
```
Before: 3-4 errors per email
After: 0-1 errors per email
Improvement: 75-100% error reduction
```

**Translation efficiency**:
```
Before: 5-10 minutes using online translator + manual editing
After: 5 seconds to translate
Savings: 99% time reduction
```

## Advanced Configurations

### Multiple API Keys (for different projects)

Currently not supported, but workaround:
1. Keep different API keys ready
2. Change in Settings when switching contexts
3. Future: Profile/project support planned

### Custom Prompts (Future Feature)

Planned feature to customize AI behavior:
```
Example custom prompts:
- "Keep technical terms unchanged"
- "Use formal/informal tone"
- "Preserve markdown formatting"
- "British vs American English"
```

## Conclusion

The key to getting the most from SpellingChecker:

1. **Use appropriately** - Right tool for the right job
2. **Review results** - AI assists but you decide
3. **Optimize workflow** - Develop efficient habits
4. **Monitor usage** - Balance quality vs cost
5. **Stay updated** - New features coming

Happy correcting and translating! 🎉
