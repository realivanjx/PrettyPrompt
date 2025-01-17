﻿#region License Header
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion

using System.Threading.Tasks;
using Xunit;
using static PrettyPrompt.Consoles.AnsiEscapeCodes;
using static System.ConsoleKey;

namespace PrettyPrompt.Tests
{
    public class SyntaxHighlightingTests
    {
        [Fact]
        public async Task ReadLine_SyntaxHighlight()
        {
            var console = ConsoleStub.NewConsole();
            console.StubInput($"red green nocolor blue{Enter}");

            var prompt = new Prompt(
                callbacks: new PromptCallbacks
                {
                    HighlightCallback = SyntaxHighlighterTestData.HighlightHandlerAsync
                },
                console: console
            );

            var result = await prompt.ReadLineAsync("> ");

            Assert.True(result.IsSuccess);
            Assert.Equal("red green nocolor blue", result.Text);
            var output = console.GetAllOutput();

            // although the words are typed character-by-character, we should still "go back" and redraw
            // it once we know the word should be drawn in a syntax-highlighted color.
            Assert.Contains(
                MoveCursorLeft("red".Length - 1) + BrightRed + "red" + Reset, // when we press 'd' go back two chars and to rewrite the word "red"
                output
            );
            Assert.Contains(
                MoveCursorLeft("green".Length - 1) + BrightGreen + "green" + Reset,
                output
            );
            Assert.Contains(
                MoveCursorLeft("blue".Length - 1) + BrightBlue + "blue" + Reset,
                output
            );

            Assert.DoesNotContain("nocolor", output); // it should output character by character as we type; never the whole string at once.
        }
    }
}
