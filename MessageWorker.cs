using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHA
{
    static class MessageWorker
    {
        public static void CalculateHash(byte[] data, bool lastBlock, long messageLength)
        {
            List<byte> content = new List<byte>(data); // Преобразуем в список байт

            if (lastBlock)
            {
                if (content.Count % 64 != 0)
                {
                    int numberOfBlocks = content.Count / 64 + 1; // Количество блоков по 512 бит

                    byte[] buffer = new byte[content.Count - content.Count / 64 * 64]; // Создаем буффер на количество байт в неполном блоке
                    for (int i = 0; i < buffer.Length; i++)
                        buffer[i] = data[content.Count / 64 * 64 + i];

                    content.RemoveRange((numberOfBlocks - 1) * 64, buffer.Length);

                    buffer = SHA.Padding(buffer, messageLength * 8);

                    foreach (byte b in buffer)
                        content.Add(b); // Записываем падинг

                    data = content.ToArray(); // Сохраняем полное сообщение
                    content.Clear();
                } // Сообщение не кратно 64 байтам (512 битам)
                else
                {
                    int numberOfBlocks = content.Count / 64; // Количество блоков по 512 бит

                    byte[] buffer = null;

                    if (content.Count != 0)
                    {
                        buffer = new byte[64]; // Создаем буффер на 64 бит
                        for (int i = 0; i < buffer.Length; i++)
                            buffer[i] = content[(numberOfBlocks - 1) * 64 + i]; // Заполняем буфер последним блоком данных

                        content.RemoveRange((numberOfBlocks - 1) * 64, 64);
                    }
                    else
                        buffer = new byte[0]; // Создаем буффер на 64 бит

                    buffer = SHA.Padding(buffer, messageLength * 8);


                    foreach (byte b in buffer)
                        content.Add(b); // Записываем падинг

                    data = content.ToArray(); // Сохраняем полное сообщение
                    content.Clear();

                } // Сообщение кратно 64 байтам (512 битам)
            } // Если принимаем последний блок сообщения, то для него выполняется падинг

            for (int i = 0; i < data.Length / 64; i++)
            {
                byte[] buffer = new byte[64];
                for (int j = 0; j < buffer.Length; j++)
                    buffer[j] = data[i * 64 + j]; // Получаем один блок сообщения на 512 бит

                SHA.CalculateHash(buffer); // Вызываем функцию для расчета хэша одного блока
            } // Для каждого блока по 512 бит вычиляем хэш функцию
        } // Функция, обеспечивает процесс шифрования сообщения
    }
}
