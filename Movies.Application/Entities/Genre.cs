namespace Movies.Application.Models {

    public class Genre {

        public Guid Id { get; set; }
        public string Name { get; set; }

        #region Navigation

        public ICollection<Movie> Movie { get; set; }

        #endregion

    }

}
