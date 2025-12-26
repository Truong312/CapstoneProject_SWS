using SWS.Test.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            int row = 6;
            int col = 5;

            TestDebug.Log("=== ReadTestCaseIds ===");

            while (true)
            {
                var tcId = _sheet.Get(row, col);
                TestDebug.Log($"Cell[{row},{col}] = '{tcId}'");

                if (string.IsNullOrEmpty(tcId)) break;

                list.Add(new TestCase { Id = tcId });
                col++;
            }

            TestDebug.Log($"Total TC IDs = {list.Count}");
            return list;
        }


        public void ReadConditions(List<TestCase> testCases)
        {
            int row = 8; // bắt đầu từ dòng "Condition" (A8 trong sheet)

            Console.WriteLine("=== START ReadConditions_DecisionTable ===");

            while (!string.Equals(
                _sheet.Get(row, 0)?.Trim(),
                "Confirm",
                StringComparison.OrdinalIgnoreCase))
            {
                var variable = _sheet.Get(row, 1)?.Trim();

                // Bỏ qua dòng tiêu đề / trống
                if (string.IsNullOrEmpty(variable) ||
                    variable.Equals("Condition", StringComparison.OrdinalIgnoreCase) ||
                    variable.Equals("Precondition", StringComparison.OrdinalIgnoreCase))
                {
                    row++;
                    continue;
                }

                Console.WriteLine($"\n[VAR] {variable}");

                int valueRow = row + 1;

                // đọc các value bên dưới variable
                while (!string.IsNullOrEmpty(_sheet.Get(valueRow, 3)))
                {
                    var value = _sheet.Get(valueRow, 3)?.Trim();

                    Console.WriteLine($"  [VALUE] row {valueRow} = {value}");

                    for (int i = 0; i < testCases.Count; i++)
                    {
                        int tcCol = 5 + i; // F +

                        var flag = _sheet.Get(valueRow, tcCol)?.Trim();

                        if (flag == "O")
                        {
                            testCases[i].Inputs[variable] = value;
                            Console.WriteLine(
                                $"    -> TC[{testCases[i].Id}] {variable} = {value}");
                        }
                    }

                    valueRow++;
                }

                // nhảy tới block variable tiếp theo
                row = valueRow;
            }

            Console.WriteLine("=== END ReadConditions_DecisionTable ===");
        }





        public void ReadConfirm(List<TestCase> testCases)
        {
            int row = FindRow("Confirm");
            TestDebug.Log($"=== ReadConfirm START at row {row} ===");

            while (true)
            {
                var colA = _sheet.Get(row, 0)?.Trim();
                if (colA == "Result")
                {
                    TestDebug.Log("Reached RESULT row");
                    break;
                }

                var type = _sheet.Get(row, 1)?.Trim();
                var value = _sheet.Get(row, 3)?.Trim();

                TestDebug.Log($"Confirm Type='{type}' Value='{value}'");

                for (int i = 0; i < testCases.Count; i++)
                {
                    int tcCol = 5 + i;
                    var flag = _sheet.Get(row, tcCol)?.Trim();

                    if (flag == "O")
                    {
                        if (type == "Return")
                            testCases[i].Expected.ReturnValue = value;

                        if (type == "Exception")
                            testCases[i].Expected.Exception = value;

                        TestDebug.Log($"  TC {testCases[i].Id} Expect {type}={value}");
                    }
                }

                row++;
            }

            TestDebug.Log("=== ReadConfirm END ===");
        }


        public int FindRow(string keyword)
        {
            for (int i = 0; i < _sheet.Count; i++)
            {
                if (_sheet.Get(i, 0) == keyword)
                    return i;
            }
            throw new Exception($"Không tìm thấy {keyword}");
        }
    }




}

