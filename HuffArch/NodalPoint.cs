using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Collections.Generic;

/*=====================[NodalPoint]=================================================
 * В этом классе содержится описание класса узловых точек дерева для создания подобных 
 * классов в классе Tree.cs.
 * Содержит: Знак, Частоту, Правую и Левую ветви.
====================================================================================*/

namespace HuffArch
{
    class NodalPoint
    {
        public char Sign { get; set; }//Знак, сюда пойдет symbol.key
        public int Frequency { get; set; }//Частоты, сюда пойдет symbol.value
        public NodalPoint RightPoint { get; set; }//Правая ветвь, для других точек при построении дерева
        public NodalPoint LeftPoint { get; set; }//Левая ветвь, для других точек при построении дерева
        //=======================[Обход дерева кодирования]=============================
        public List<bool> ByPassTree(char symbol, List<bool> data)//На вход символ и лист c битами
        {
          
                if (LeftPoint == null && RightPoint == null)//Правая и левая пусты
                {
                    if (symbol.Equals(this.Sign)) return data;//Если совпадает вернуть последовательность TrueFalse
                    else return null;//иначе вернуть пустую ссылку
                }
                else//Если правая или левая не пусты
                {
                    List<bool> leftPoint = null;//Создать булевый лист левых узловых точек, нужный для спуска вниз по дереву
                    List<bool> rightPoint = null;//Создать булевый лист правых узловых точек, нужный для спуска вниз по дереву.
                    if (LeftPoint != null)//Левая точка не пуста
                    {
                        List<bool> leftPointKey = new List<bool>();//Создадим лист кодирования/декодирования знака для левых точек
                        leftPointKey.AddRange(data);//Добавить в конец списка последовательность TrueFalse
                        leftPointKey.Add(false);//Добавить элемент false (0)
                        leftPoint = LeftPoint.ByPassTree(symbol, leftPointKey);//Начнем обход левой точки
                    }
                    if (RightPoint != null)//Правая точка не пуста
                    {
                        List<bool> rightPointKey = new List<bool>();//Создадим лист кодирования/декодирования знака для правых точек
                        rightPointKey.AddRange(data);//Добавить в конец списка последовательность TrueFalse
                        rightPointKey.Add(true);//Добавить элемент true (1)
                        rightPoint = RightPoint.ByPassTree(symbol, rightPointKey);//Начнем обход правой точки
                    }
                    if (leftPoint != null) return leftPoint;
                    else return rightPoint;
                }
         
        }
    }
}
