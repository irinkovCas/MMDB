namespace Movies.Application.Models;

public class Rating
{

    public Guid MovieId { get; set; }
    public Guid Id { get; set; }
    public float Score { get; set; }

    public Guid UserId { get; set; }

    #region Navigation Properties

    public Movie Movie { get; set; }

    #endregion

}
