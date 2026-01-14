using SWS.Test.Models;
using System;
using System.Collections.Generic;

namespace SWS.Test.Helpers
{
    public class SheetParser
    {
        private readonly IList<IList<object>> _sheet;

        public SheetParser(IList<IList<object>> sheet)
        {
            _sheet = sheet;
        }

        public List<TestCase> ReadTestCases()
        {
            var testCases = ReadTestCaseIds();
            ReadConditions(testCases);
            ReadConfirm(testCases);
            return testCases;
        }

        public List<TestCase> ReadTestCaseIds()
        {
            var list = new List<TestCase>();
            int row = 6; // dòng TC ID
            int col = 5; // cột F

            TestDebug.Log("=== ReadTestCaseIds ===");

            while (true)
            {
                var tcId = _sheet.Get(row, col);

                if (string.IsNullOrWhiteSpace(tcId))
                    break;

                list.Add(new TestCase
                {
                    Id = tcId,
                    ColumnIndex = col // ⭐ QUAN TRỌNG
                });

                col++;
            }

            TestDebug.Log($"Total TC IDs = {list.Count}");
            return list;
        }

        public void ReadConditions(List<TestCase> testCases)
        {
            int row = 8;

            while (!EqualsIgnoreCase(_sheet.Get(row, 0), "Confirm"))
            {
                var variable = _sheet.Get(row, 1)?.Trim();

                if (string.IsNullOrEmpty(variable) ||
                    EqualsIgnoreCase(variable, "Condition") ||
                    EqualsIgnoreCase(variable, "Precondition"))
                {
                    row++;
                    continue;
                }

                int valueRow = row + 1;

                while (!string.IsNullOrEmpty(_sheet.Get(valueRow, 3)))
                {
                    var value = _sheet.Get(valueRow, 3)?.Trim();

                    for (int i = 0; i < testCases.Count; i++)
                    {
                        int tcCol = testCases[i].ColumnIndex;
                        var flag = _sheet.Get(valueRow, tcCol)?.Trim();

                        if (flag == "O")
                            testCases[i].Inputs[variable] = value;
                    }

                    valueRow++;
                }

                row = valueRow;
            }
        }

        public void ReadConfirm(List<TestCase> testCases)
        {
            int row = FindRow(_sheet, "Confirm") + 1;

            while (row < _sheet.Count)
            {
                if (EqualsIgnoreCase(_sheet.Get(row, 0), "Result"))
                    break;

                var type = _sheet.Get(row, 1)?.Trim();
                var value = _sheet.Get(row, 3)?.Trim();

                for (int i = 0; i < testCases.Count; i++)
                {
                    int tcCol = testCases[i].ColumnIndex;
                    var flag = _sheet.Get(row, tcCol)?.Trim();

                    if (flag == "O")
                    {
                        if (type == "Return")
                            testCases[i].Expected.ReturnValue = value;
                        else if (type == "Exception")
                            testCases[i].Expected.Exception = value;
                    }
                }

                row++;
            }
        }

        public static int FindRow( IList<IList<object>> sheet, string keyword, int colIndex, int startRow = 0)
        {
            for (int r = startRow; r < sheet.Count; r++)
            {
                if (sheet[r].Count <= colIndex)
                    continue;

                var cell = sheet[r][colIndex];
                if (cell == null)
                    continue;

                var text = cell.ToString()?.Trim();
                if (string.Equals(text, keyword, StringComparison.OrdinalIgnoreCase))
                    return r;
            }

            throw new Exception($"Row '{keyword}' not found");
        }

        public static int TryFindRow(IList<IList<object>> sheet, string keyword, int colIndex = 0, int startRow = 0)
        {
            for (int i = startRow; i < sheet.Count; i++)
            {
                var cell = GetCell(sheet, i, colIndex);
                if (!string.IsNullOrEmpty(cell) && cell.Trim().Equals(keyword, StringComparison.OrdinalIgnoreCase))
                    return i;
            }
            return -1;
        }



        private static bool EqualsIgnoreCase(object? value, string keyword)
        {
            return value?.ToString()?.Trim()
                .Equals(keyword, StringComparison.OrdinalIgnoreCase) == true;
        }

        public static int FindRow( IList<IList<object>> sheet, string keyword)
        {
            return FindRow(sheet, keyword, 0, 0);
        }

        public static string? GetCell(IList<IList<object>> sheet, int row, int col)
        {
            if (row < 0 || row >= sheet.Count)
                return null;

            if (col < 0 || col >= sheet[row].Count)
                return null;

            return sheet[row][col]?.ToString();
        }
        public static void SetCell(IList<IList<object>> sheet, int row, int col, string? value)
        {
            while (sheet[row].Count <= col)
                sheet[row].Add("");

            sheet[row][col] = value ?? "";
        }

        public static (int confirmRow, int returnRow, int exceptionRow)
    FindConfirmBlock(IList<IList<object>> sheet)
        {
            int confirmRow = -1;
            int returnRow = -1;
            int exceptionRow = -1;

            for (int r = 0; r < sheet.Count; r++)
            {
                var colA = sheet.Get(r, 0)?.Trim();
                if (EqualsIgnoreCase(colA, "Confirm"))
                {
                    confirmRow = r;
                    break;
                }
            }

            if (confirmRow < 0)
                throw new Exception("Confirm block not found");

            for (int r = confirmRow; r < sheet.Count; r++)
            {
                var colA = sheet.Get(r, 0)?.Trim();
                if (EqualsIgnoreCase(colA, "Result"))
                    break;

                var colB = sheet.Get(r, 1)?.Trim();
                if (EqualsIgnoreCase(colB, "Return"))
                    returnRow = r;
                else if (EqualsIgnoreCase(colB, "Exception"))
                    exceptionRow = r;
            }

            if (returnRow < 0)
                throw new Exception("Return row not found inside Confirm block");

            return (confirmRow, returnRow, exceptionRow);
        }

    }
}
