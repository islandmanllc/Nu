﻿using System;
using System.Drawing;
using System.Linq;
using ScintillaNET;
using ScintillaNET_FindReplaceDialog;
using System.Windows.Forms;

namespace Nu.Gaia.Design
{
    public class SymbolicTextBox : Scintilla
    {
        public SymbolicTextBox()
        {
            // Make default styles monospaced!
            Styles[Style.Default].Font = "Lucida Console";

            // Add a little more line spacing for new font
            ExtraDescent = 1;

            // Lisp lexer
            Lexer = Lexer.Lisp;

            // Add keyword styles (keywords 0 are reserved for DSL-specific use)
            Styles[Style.Lisp.Keyword].ForeColor = Color.DarkBlue;
            Styles[Style.Lisp.KeywordKw].ForeColor = Color.FromArgb(0xFF, 0x60, 0x00, 0x70);
            Keywords1 = "True False Some None Right Left Nor Not Nand Mod Xor And Mul Add Sub Eq Or Lt Gt Get Div Not_Eq Gt_Eq Lt_Eq";

            // Add operator styles (braces, actually)
            Styles[Style.Lisp.Operator].ForeColor = Color.RoyalBlue;
            Styles[Style.BraceLight].BackColor = Color.LightBlue;
            Styles[Style.BraceBad].BackColor = Color.Red;

            // Add symbol styles (operators, actually)
            Styles[Style.Lisp.Special].ForeColor = Color.DarkBlue;

            // Add string style
            Styles[Style.Lisp.String].ForeColor = Color.Teal;

            // No tabs
            UseTabs = false;

            // Implement brace matching
            UpdateUI += SymbolicTextBox_UpdateUI;

            // Implement auto-complete
            CharAdded += SymbolicTextBox_CharAdded;

            // Implement find/replace
            MyFindReplace = new FindReplace(this);
            KeyDown += SymbolicTextBox_KeyDown;
        }

        public string Keywords0
        {
            get { return keywords0; }
            set
            {
                keywords0 = value;
                SetKeywords(0, keywords0);
            }
        }

        public string Keywords1
        {
            get { return keywords1; }
            set
            {
                keywords1 = value;
                SetKeywords(1, keywords1);
            }
        }

        public string AutoCWords
        {
            get
            {
                var keywordsSplit = keywords0.Split(' ').Distinct().ToArray();
                Array.Sort(keywordsSplit);
                var keywordsSorted = string.Join(AutoCSeparator.ToString(), keywordsSplit);
                return keywordsSorted;
            }
        }

        private void SymbolicTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && !e.Shift && e.KeyCode == Keys.F)
            {
                MyFindReplace.ShowIncrementalSearch();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && !e.Shift && e.KeyCode == Keys.H)
            {
                MyFindReplace.ShowReplace();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.Shift && e.KeyCode == Keys.F)
            {
                MyFindReplace.ShowFind();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.Shift && e.KeyCode == Keys.H)
            {
                MyFindReplace.ShowReplace();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.G)
            {
                GoTo MyGoTo = new GoTo(this);
                MyGoTo.ShowGoToDialog();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.F3)
            {
                // TODO: figure out how to call this from here
                // MyFindReplace.FindNext();
                // e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.Escape)
            {
                MyFindReplace.ClearAllHighlights();
                e.SuppressKeyPress = true;
            }
        }

        private void SymbolicTextBox_CharAdded(object sender, CharAddedEventArgs e)
        {
            // Find the word start
            var currentPos = CurrentPosition;
            var wordStartPos = WordStartPosition(currentPos, true);

            // Display the autocompletion list
            var lenEntered = currentPos - wordStartPos;
            if (lenEntered > 0) AutoCShow(lenEntered, AutoCWords);
        }

        private void SymbolicTextBox_UpdateUI(object sender, UpdateUIEventArgs e)
        {
            // Has the selection changed position?
            var selectionPos = SelectionStart;
            if (lastSelectionPos != selectionPos)
            {
                lastSelectionPos = selectionPos;
                var bracePos1 = -1;
                var bracePos2 = -1;
                if (IsBrace(GetCharAt(selectionPos)))
                {
                    // Select the brace to the immediate right
                    bracePos1 = selectionPos;
                }

                if (bracePos1 >= 0)
                {
                    // Find the matching brace
                    bracePos2 = BraceMatch(bracePos1);
                    if (bracePos2 == InvalidPosition)
                    {
                        BraceBadLight(bracePos1);
                        HighlightGuide = 0;
                    }
                    else
                    {
                        BraceHighlight(bracePos1, bracePos2);
                        HighlightGuide = GetColumn(bracePos1);
                    }
                }
                else
                {
                    // Turn off brace matching
                    BraceHighlight(InvalidPosition, InvalidPosition);
                    HighlightGuide = 0;
                }
            }
        }

        private bool IsBrace(int c)
        {
            switch (c)
            {
                case '(':
                case ')':
                case '[':
                case ']':
                case '{':
                case '}': return true;
            }
            return false;
        }

        private string keywords0 = string.Empty;
        private string keywords1 = string.Empty;
        private int lastSelectionPos = 0;
        private FindReplace MyFindReplace;
    }
}