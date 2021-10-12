using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labeling
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("読み込むﾌｧｲﾙ名を入力");
            string fileName = Console.ReadLine();

            CsvParser csv = new CsvParser(fileName);
            List<string[]> labeled = new List<string[]>();
            Dictionary<string, string> lookup = new Dictionary<string, string>();
            int labelCount = 0;
            
            foreach (var line in csv.GetLine())
            {
                labeled.Add(line);

                //とりあえず表示しとく
                foreach (var cell in line)
                {
                    Console.Write(cell.PadLeft(3));
                }
                Console.WriteLine();
            }

            //ラベリング1回目
            for (int y = 0; y < labeled.Count; y++)
            {
                for (int x = 0; x < labeled[y].Length; x++)
                {
                    //自身がラベリング対象でないなら何もしない。
                    if (labeled[y][x] == "0") continue;

                    string labelLeft = "";
                    if (x - 1 >= 0)
                    {
                        //左がラベリングされていたらそれに合わせる。
                        labelLeft = labeled[y][x - 1];
                        if (labelLeft != "0")
                        {
                            labeled[y][x] = labelLeft;
                        }
                    }

                    string labelUp = "";
                    if (y - 1 >= 0)
                    {
                        //上がラベリングされていたらそれに合わせる。
                        labelUp = labeled[y - 1][x];
                        if (labelUp != "0")
                        {
                            labeled[y][x] = labelUp;
                        }
                    }

                    if ((string.IsNullOrEmpty(labelUp) || labelUp.Equals("0")) && (string.IsNullOrEmpty(labelLeft) || labelLeft.Equals("0")))
                    {
                        //隣接ラベルが無ければ新たなラベルを貼る。
                        labelCount += 1;
                        labeled[y][x] = labelCount.ToString();
                        continue;
                    }

                    //両方の隣接ラベルが無い。
                    if (string.IsNullOrEmpty(labelUp) || labelUp.Equals("0") || string.IsNullOrEmpty(labelLeft) || labelLeft.Equals("0"))
                    {
                        continue;
                    }

                    //縦横に隣接ラベルがあるならルックアップテーブルに格納
                    if (int.Parse(labelUp) > int.Parse(labelLeft))
                    {
                        string insert = labelLeft;
                        //既にテーブルにあったら、その値と比較して小さい方を格納
                        if (lookup.ContainsKey(labelUp))
                        {
                            if (int.Parse(lookup[labelUp]) < int.Parse(insert)) insert = lookup[labelUp];
                        }
                        lookup[labelUp] = insert;
                    }
                    else if (int.Parse(labelLeft) > int.Parse(labelUp))
                    {
                        string insert = labelUp;
                        //既にテーブルにあったら、その値と比較して小さい方を格納
                        if (lookup.ContainsKey(labelLeft))
                        {
                            if (int.Parse(lookup[labelLeft]) < int.Parse(insert)) insert = lookup[labelLeft];
                        }
                        lookup[labelLeft] = insert;
                    }
                    else
                    {
                        //隣接ラベルがどっちも同じラベル
                        lookup[labelUp] = labelLeft;
                    }
                }
            }

            //ラベリングした途中結果を表示
            Console.WriteLine("ラベリング1終了");
            for (int y = 0; y < labeled.Count; y++)
            {
                for (int x = 0; x < labeled[y].Length; x++)
                {
                    Console.Write(labeled[y][x].PadLeft(3));
                }
                Console.WriteLine();
            }

            Console.WriteLine("領域数は " + (labelCount - lookup.Count) + " です");

            List<string> adjust = new List<string>();
            //ルックアップテーブルの書き戻し
            for (int y = 0; y < labeled.Count; y++)
            {
                for (int x = 0; x < labeled[y].Length; x++)
                {
                    string label = labeled[y][x];
                    if (lookup.ContainsKey(label))
                    {
                        labeled[y][x] = lookup[label];
                    }

                    if (adjust.Contains(labeled[y][x])) continue;
                    adjust.Add(labeled[y][x]);
                }
            }

            //詰める
            adjust = adjust.OrderBy(x => x).ToList();
            Dictionary<string, string> push = new Dictionary<string, string>();
            for (int i = 0; i < adjust.Count; i++)
            {
                push[adjust[i]] = i.ToString();
            }

            //ラベリングした結果を表示
            Console.WriteLine("ラベリング結果");
            for (int y = 0; y < labeled.Count; y++)
            {
                for (int x = 0; x < labeled[y].Length; x++)
                {
                    Console.Write(push[labeled[y][x]].PadLeft(3));
                }
                Console.WriteLine();
            }

            Console.WriteLine("Y: 続行  N:終了");
            switch (Console.ReadKey().KeyChar)
            {
                case 'y':
                    Main(args);
                    break;
                case 'n':
                    break;
            }
        }
    }
}
