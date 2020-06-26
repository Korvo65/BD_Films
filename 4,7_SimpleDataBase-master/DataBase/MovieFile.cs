using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public class FilmFile
    {
        string producerName; // имя режисера 
        string movieTitle; // название фильма
        ushort yearRelease; // год выпуска фильма
        string movieGenre; // жанр фильма
        ushort movieid; // id кино

        public FilmFile(ushort movieID, string producerName, string movieTitle, ushort yearRelease, string movieGenre)
        {
            if ((producerName == "") || (movieTitle == "") || (movieGenre == ""))
                throw new Exception("Все поля должны быть заполнены!");
            this.producerName = producerName;
            this.movieTitle = movieTitle;
            this.movieGenre = movieGenre;
            if ((yearRelease < 1900) || (yearRelease > DateTime.Now.Year))
                throw new Exception("Год выпуска не раньше 1900 и не позднее " + DateTime.Now.Year);
            this.yearRelease = yearRelease;
            this.movieid = movieID;
        }

        public string ProducertName
        {
            get
            {
                return producerName;
            }

            set
            {
                producerName = value;
            }
        }

        public string MovieTitle
        {
            get
            {
                return movieTitle;
            }

            set
            {
                movieTitle = value;
            }
        }

        public ushort YearRelease
        {
            get
            {
                return yearRelease;
            }

            set
            {
                yearRelease = value;
            }
        }

        public string KinoGenre
        {
            get
            {
                return movieGenre;
            }

            set
            {
                movieGenre = value;
            }
        }

        public ushort MovieID
        {
            get
            {
                return movieid;
            }
            set
            {
                movieid = value;
            }
        }

        public override string ToString()
        {
            return movieid + "|" + producerName + "|" + movieTitle + "|" +
                yearRelease + "|" + movieGenre;
        }
    }
}
