using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace HuffArch
{
    class Tree
    {
        public Dictionary<char, int> Frequency = new Dictionary<char, int>();//Словарь частот
        [NonSerialized]
        private List<NodalPoint> nodes = new List<NodalPoint>();//Лист для узлов
        public NodalPoint Root { get; set; }
        public void Build(char[] input, string adress, bool key, Dictionary<char, int> recollectionTabel)//Массив чар, адрес для сериализации, ключ прохода, востановленный словарь.
        {

            //===============[Построение/восстановление таблицы и листа с узлами]=====================
            if (key == true)//При новом файле
            {
                for (int i = 0; i < input.Length; i++)//подсчет символов
                {
                    if (!Frequency.ContainsKey(input[i]))//Проверка на наличие в словаре
                    {
                        Frequency.Add(input[i], 0);//Добавление в словарь    
                    }
                    Frequency[input[i]]++;//Прибавляем показатель частоты встречаемости
                }
                foreach (KeyValuePair<char, int> symbol in Frequency)//Перебор, создание списка узлов для дерева, которые берем из словаря
                {
                    nodes.Add(new NodalPoint() { Sign = symbol.Key, Frequency = symbol.Value });
                }
                try
                {
                    FileStream saveTable = new FileStream(adress, FileMode.Create);
                    BinaryFormatter bitform = new BinaryFormatter();
                    bitform.Serialize(saveTable, Frequency); // сохранение объекта  в потоке f
                    saveTable.Close();
                }
                catch (ArgumentException e) { }
            }
            else//При наличии востановленной таблицы, т.е. при разархивации.
            {
                foreach (KeyValuePair<char, int> symbol in recollectionTabel)//Перебор символов в таблице, создание списка узлов дерева
                {
                    nodes.Add(new NodalPoint() { Sign = symbol.Key, Frequency = symbol.Value });//В знак запишем символ, в частоту - значение частоты из таблицы, Правую и левую ветвь пока пустыми
                }

            }
           
            
                //=========================[Построение дерева кодирования]=================================
                while (nodes.Count > 1)//Пока есть хотябы 2 узла в листе
                {
                    List<NodalPoint> orderNodesOfFreq = nodes.OrderBy(node => node.Frequency).ToList<NodalPoint>();//Сортировка в порядке возрастания частотности
                    if (orderNodesOfFreq.Count >= 2)//Если узлов больше 2
                    {
                        List<NodalPoint> take = orderNodesOfFreq.Take(2).ToList<NodalPoint>();//Взять два пункта с начала и записать в лист взятых
                        NodalPoint parentRoot = new NodalPoint()//Создать узел родитель
                        {
                            Sign = '*',
                            Frequency = take[0].Frequency + take[1].Frequency,
                            LeftPoint = take[0],
                            RightPoint = take[1]
                            //Знак,              сумма частот,                              первый в левую ветвь,  второй в правую
                        };
                        nodes.Remove(take[0]);//Удаляем первый взятый элемент
                        nodes.Remove(take[1]);//Удаляем второй взятый элемент
                        nodes.Add(parentRoot);//Добавляем новый элемент, ставший корнем дерева для двух взятых.
                    }
                    this.Root = nodes.FirstOrDefault();//Сделать корнем дерева первый элемент
                }//while (nodes.Count > 1)
    
               }
        public BitArray Encode(char[] input)//Массив символов
        {
           
                List<bool> encodBoolTable = new List<bool>();//Заготовка для массива бит
                int check = input.Length;
                int n = 1;               
                for (int i = 0; i < input.Length; i++)//Перебор входящего массива для кодирования
                {
                    List<bool> encodBoolSymbol = this.Root.ByPassTree(input[i], new List<bool>());//Сформируем лист кодировки символа с помощью обхода дерева
                    encodBoolTable.AddRange(encodBoolSymbol);//Добовляем биты в виде True/False по символу из переменной input
                    if (i == check * n / 12)
                    {
                         switch (n)
                         {
                             case 1:
                                 Console.Write("[**");
                                 break;
                             case 11:
                                 Console.WriteLine("**]");
                                 break;
                             default:
                                 Console.Write("**");
                                 break;
                         }
                    n += 1;
                    
                    }
                }
                BitArray masBits = new BitArray(encodBoolTable.ToArray());//Перенос данных в класс-массив BitArray
                return masBits;//Возврат класса-массива bits
         
       
        }
        //=============================[Декодирование]===============================
        public string Decode(BitArray masBits)//Массив бит
        {
                string decodedText = string.Empty;//Обнуление выходных данных
                int check = masBits.Length;
                int n = 1;
                int i = 0;
            NodalPoint actual = this.Root;
                foreach (bool bit in masBits)//Перебор бит в виде bool в массиве Bits
                {
                    if (bit)//Бит true, т.е. 1
                    {
                        if (actual.RightPoint != null) actual = actual.RightPoint;//Если справа не пусто, заходим в право   
                    }
                    else//Бит false, т.е. 0     
                    {
                        if (actual.LeftPoint != null) actual = actual.LeftPoint;//Если слева не пусто, заходим в лево     
                    }
                    if (actual.LeftPoint == null && actual.RightPoint == null)//Если обе ветви пусты, то 
                    {
                        decodedText += actual.Sign;//Добавление к последовательности раскодированных символов знака
                        actual = this.Root;//Вернем к корню дерева для следующего прохода
                    }
                    i++;
                    if (i == check * n / 12)
                    {
                           switch (n)
                          {
                               case 1:
                                    Console.Write("[**");
                                    break;
                              case 11:
                                  Console.WriteLine("**]");
                                 break;
                               default:
                                  Console.Write("**");
                                  break;
                          }
                      n += 1;
                    }
            }
                return decodedText;//Возврат декодированной последовательности
        }
    }
}


