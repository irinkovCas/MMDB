namespace Movies.Contracts.Entities {

    public class RatingDto {

        public Guid MovieId { get; set; }
        public float Rating { get; set; }
        public Guid UserId { get; set; }

    }

}
