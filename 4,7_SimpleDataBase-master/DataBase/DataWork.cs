using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace DataBase
{
    public class DataWork
    {
        ArrayList filmFiles = new ArrayList();

       
        /// Вернуть коллекцию
       
        public ArrayList FilmFiles
        {
            get
            {
                return filmFiles;
            }
        }

        
        /// Добавление фильма в коллекцию
   
        public void AddMusicFile(FilmFile musicFile)
        {
            filmFiles.Add(musicFile);
        }

      
        /// Удаление всей коллекции
       
        public void DeleteMusic() => filmFiles.Clear();

    
        /// Удаление элемента коллекции по индексу
     
        public void DeleteFilmFile(int number) => filmFiles.RemoveAt(number);

        
        /// Изменить рижесера у заданного элемента
        
        public void ChangeArtistName(string artist, int index)
        {
            FilmFile film = (FilmFile)FilmFiles[index];
            film.ProducertName = artist;
        }

        
        /// Изменить название фильма у заданного элемента
       
        public void ChangeSongTittle(string movie, int index)
        {
            FilmFile film = (FilmFile)FilmFiles[index];
            film.MovieTitle = movie;
        }

        
        /// Изменить год релиза фильма у заданного элемента
        
        public void ChangeYearRelease(ushort year, int index)
        {
            FilmFile music = (FilmFile)FilmFiles[index];
            if ((year < 1900) || (year > DateTime.Now.Year))
                throw new Exception("Год выпуска не раньше 1900 и не позднее " + DateTime.Now.Year);
            music.YearRelease = year;
        }

        
        /// Изменить жанр песни у заданного элемента
        
        public void ChangeGenreSong(string genre, int index)
        {
            FilmFile music = (FilmFile)FilmFiles[index];
            music.KinoGenre = genre;
        }

      
        /// Сохранение коллекции в файл
        
        public void SaveToFile(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename, false, System.Text.Encoding.Unicode))
            {
                foreach (FilmFile s in filmFiles)
                {
                    sw.WriteLine(s.ToString());
                }
            }
        }

        
        /// Восстанавливает коллекцию, записанную в файл
        
        public void OpenFile(string filename)
        {
            if (!System.IO.File.Exists(filename))
                throw new Exception("Файл не существует");
            if (filmFiles.Count != 0)
                DeleteMusic();
            using (StreamReader sw = new StreamReader(filename))
            {
                while (!sw.EndOfStream)
                {
                    string str = sw.ReadLine();
                    String[] dataFromFile = str.Split(new String[] { "|" },
                        StringSplitOptions.RemoveEmptyEntries);
                    ushort id = (ushort)Convert.ToInt32(dataFromFile[0]);
                    string artist = dataFromFile[1];
                    string song = dataFromFile[2];
                    ushort year = (ushort)Convert.ToInt32(dataFromFile[3]);
                    string genre = dataFromFile[4];
                    FilmFile movieFile = new FilmFile(id, artist, song, year, genre);
                    AddMusicFile(movieFile);
                }
            }
        }

        /// Поиск по заданному параметру и возвращение индексов найденных элементов
        /// Вернет -1, если элементы не найдены
       
        public List<int> SearchMusicFile(string query)
        {
            List<int> count = new List<int>();
            ushort num_query;
            if (ushort.TryParse(query, out num_query))
            {
                for (int i = 0; i < filmFiles.Count; i++)
                {
                    FilmFile music = (FilmFile)filmFiles[i];
                    if (music.MovieID == num_query)
                    {
                        count.Add(i);
                        break;
                    }
                    else
                    {
                        if (music.YearRelease == num_query)
                            count.Add(i);
                    }
                }
                if (count.Count == 0)
                    count.Add(-1);
                return count;
            }
            query = query.ToLower(); // перевод в нижний регистр
            query = query.Replace(" ", "");
            for (int i = 0; i < filmFiles.Count; i++)
            {
                FilmFile movie = (FilmFile)filmFiles[i];
                if (movie.ProducertName.ToLower().Replace(" ", "").Contains(query))
                    count.Add(i);
                else
                    if (movie.MovieTitle.ToLower().Replace(" ", "").Contains(query))
                    count.Add(i);
                else
                    if (movie.KinoGenre.ToLower().Replace(" ", "").Contains(query))
                    count.Add(i);
            }
            if (count.Count == 0)
                count.Add(-1);
            return count;
        }

        public void Sort(SortDirection direction)
        {
            filmFiles.Sort(new YearComparer(direction));
        }
    }

    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public class YearComparer : IComparer
    {
        private SortDirection m_direction = SortDirection.Ascending;

        public YearComparer() : base() { }

        public YearComparer(SortDirection direction)
        {
            this.m_direction = direction;
        }

        int IComparer.Compare(object x, object y)
        {
            FilmFile film1 = (FilmFile)x;
            FilmFile film0 = (FilmFile)y;

            return (this.m_direction == SortDirection.Ascending) ?
                film1.YearRelease.CompareTo(film0.YearRelease) :
                film0.YearRelease.CompareTo(film1.YearRelease);
        }
    }
}
