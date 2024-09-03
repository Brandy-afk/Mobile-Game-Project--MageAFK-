using Backend.Data;
using Backend.Domain.Modals;
using HotChocolate.Resolvers;

namespace Backend.Domain.Types.Many
{
    public class LeaderBoardObjectType : ObjectType<Leaderboard>
    {
        protected override void Configure(IObjectTypeDescriptor<Leaderboard> descriptor)
        {
            descriptor
                .Description("Represents a leaderboard entry for a player at a specific location");

            descriptor
                .Field(l => l.ID)
                .Description("The unique identifier of the leaderboard entry")
                .Type<NonNullType<IntType>>();

            descriptor
                .Ignore(l => l.ID);

            descriptor
                .Field(l => l.LocationID)
                .Description("The ID of the location associated with this leaderboard entry")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(l => l.Wave)
                .Description("The wave achieved by the player for this leaderboard entry")
                .Type<NonNullType<IntType>>();

            descriptor
                .Field(l => l.Created)
                .Description("The date and time when this leaderboard entry was created")
                .Type<NonNullType<DateTimeType>>();

            descriptor
             .Field("username")
             .Description("The username of the player")
             .Type<NonNullType<StringType>>()
             .Resolve(async (context) =>
             {
                 var leaderboard = context.Parent<Leaderboard>();
                 var dbContext = context.Service<GameDbContext>();

                 // Assuming you have a Users table in your other database
                 var user = await dbContext.Players.FindAsync(leaderboard.PlayerID);
                 return user?.Username ?? "Unknown";
             });
        }

   
    }
}
