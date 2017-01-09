using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;

namespace HuffArch
{
    class Program
    {
        static void Main(string[] args)
        {
            string zpunzp = args[0];
            string inputFile = args[1]; 
            string outputFile = args[2];
            switch (zpunzp)
            {
                case "-zip":
                    Console.WriteLine("Zipping process started!...");
                    if (!File.Exists(inputFile))
                    {
                        Console.WriteLine("File \"" + inputFile + "\" doesn't exist!");
                        Console.ReadKey();
                        return;
                    }
                    char[] input = (File.ReadAllText(inputFile)).ToCharArray();//Считывание
                    Tree huffmanTree = new Tree();
                    huffmanTree.Build(input, outputFile, true, null);//Построение дерева
                    BitArray encoded = huffmanTree.Encode(input);//Кодирование, на выходе класс бит

                    FileStream saveEncodedFile = new FileStream(outputFile, FileMode.Append);//Откроем файл с сохраненной таблицей
                    BinaryFormatter bitform = new BinaryFormatter();
                    bitform.Serialize(saveEncodedFile, encoded); // сохранение объекта в файл
                    saveEncodedFile.Close();//Закрытие потока

                    FileInfo FId = new FileInfo(inputFile);
                    long fileMas = FId.Length;//Размер файла в байтах
                    FileInfo FIp = new FileInfo(outputFile);
                    long fileMas1 = FIp.Length;//Размер файла в байтах
                    long percent = 100 - fileMas1 * 100 / fileMas;//Получение процентного выйгрыша в памяти
                                                                  //---
                    Console.WriteLine("File zipped:" + Environment.NewLine + "Initial size: " + fileMas / 1024 + " Kb" + Environment.NewLine + "Finish size: "
                        + fileMas1 / 1024 + " Kb" + Environment.NewLine + "Compression ratio: " + percent + " %");
                    Console.Read();
                    break;
                case "-unzip":
                    Console.WriteLine("Unzipping process started!...");
                    FileStream openEncodedFile = new FileStream(inputFile, FileMode.Open);
                    BinaryFormatter bf = new BinaryFormatter();
                    try
                    {
                        Dictionary<char, int> recollectionTabel = (Dictionary<char, int>)bf.Deserialize(openEncodedFile); //восстановление таблицы    
                        Tree huffmanTree1 = new Tree();//Создание дерева
                        huffmanTree1.Build(null, inputFile, false, recollectionTabel);//Восстанавливаем дерево.(false - декодирование, т.е. восстановление таблицы)                           
                        BitArray encoded1 = (BitArray)bf.Deserialize(openEncodedFile); //Восстановление файла
                        string decodedText = huffmanTree1.Decode(encoded1);
                        File.WriteAllText(outputFile, decodedText);
                        openEncodedFile.Close();
                        Console.WriteLine("File: " + inputFile + " unzipped to " + outputFile + Environment.NewLine);
                    }
                    catch (System.Runtime.Serialization.SerializationException e)
                    {
                        Console.WriteLine("Vladislav Olegovich ne beyte, some problems with decoding Unicode pairs :))");
                    }           
                    Console.Read();
                    break;
            }
            
        }
    }
}
