using Backend.Domain.Modals;
using Backend.Domain.Modals.Currency;
using Backend.Domain.Modals.Milestones;
using Backend.Domain.Modals.PlayerStatistics;
using Backend.Domain.Modals.Recipes;
using Backend.Domain.Modals.Skills;
using Backend.Domain.Modals.Spells;
using Backend.Payloads;
using Backend.Repositories.Interfaces;

namespace Backend.Queries
{
    public class Query
    {

        private readonly ILogger<Query> _logger;

        public Query(ILogger<Query> logger)
        {
            _logger = logger;
        }


        #region Currency

        [GraphQLDescription("Get a specific currency for a player")]
        public async Task<Currency?> GetCurrency([Service] ICurrencyRepository repository, int playerId, int typeId)
        {
            try
            {
                var currency = await repository.FindAsync(playerId, typeId);
                if (currency == null)
                {
                    _logger.LogError("Failed to get currency for player {PlayerId} and type {TypeId}", playerId, typeId);
                }
                return currency;
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get currency for player {PlayerId} and type {TypeId}", playerId, typeId);
                return null;
            }
        }

        [GraphQLDescription("Get all currencies for a player")]
        public async Task<IEnumerable<Currency>?> GetPlayerCurrencies([Service] ICurrencyRepository repository, int playerId)
        {
            try
            {
                var currencies = await repository.GetMultipleAsync(playerId);
                _logger.LogInformation("Retrieved {Count} currencies for player {PlayerId}", currencies?.Count(), playerId);
                 return currencies;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get currencies for player {PlayerId}", playerId);
                return null;
            }
        }


        #endregion

        #region Locations

        [GraphQLDescription("Get a specific location data for a player")]
        public async Task<Domain.Modals.Location.Location?> GetLocation([Service] ILocationRepository repository, int playerId, int typeId)
        {
            try
            {
               var location = await repository.FindAsync(playerId, typeId);
               if (location == null)
                {
                    _logger.LogError("Failed to get location for player {PlayerId} and type {TypeId}", playerId, typeId);
                }
                return location;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get location for player {PlayerId} and type {TypeId}", playerId, typeId);
                return null;
            }
        }

        [GraphQLDescription("Get all location data for a player")]
        public async Task<IEnumerable<Domain.Modals.Location.Location>?> GetPlayerLocations([Service] ILocationRepository repository, int playerId)
        {
            try
            {
                var locations = await repository.GetMultipleAsync(playerId);
                _logger.LogInformation("Retrieved {Count} locations for player {PlayerId}", locations?.Count(), playerId);
                return locations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get locations for player {PlayerId}", playerId);
                return null;
            }
        }

        #endregion

        #region Milestones

        [GraphQLDescription("Get a specific milestone for a player")]
        public async Task<Milestone?> GetMilestone([Service] IMilestoneRepository repository, int playerId, int typeId)
        {
            try
            {
                var milestone = await repository.FindAsync(playerId, typeId);
                if (milestone == null)
                {
                    _logger.LogInformation("Milestone not found for player {PlayerId} and type {TypeId}", playerId, typeId);
                }
                return milestone;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get milestone for player {PlayerId} and type {TypeId}", playerId, typeId);
                return null;
            }
        }

        [GraphQLDescription("Get all milestones for a player")]
        public async Task<IEnumerable<Milestone>?> GetPlayerMilestones([Service] IMilestoneRepository repository, int playerId)
        {
            try
            {
                var milestones = await repository.GetMultipleAsync(playerId);
                _logger.LogInformation("Retrieved {Count} milestones for player {PlayerId}", milestones?.Count(), playerId);
                return milestones;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get milestones for player {PlayerId}", playerId);
                return null;
            }
        }


        #endregion

        #region Statistics

        [GraphQLDescription("Get a specific statistic for a player")]
        public async Task<Statistic?> GetStatistic([Service] IStatisticRepository repository, int playerId, int typeId)
        {
            try
            {
                var statistic = await repository.FindAsync(playerId, typeId);
                if (statistic == null)
                {
                    _logger.LogInformation("Statistic not found for player {PlayerId} and type {TypeId}", playerId, typeId);
                }
                return statistic;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get statistic for player {PlayerId} and type {TypeId}", playerId, typeId);
                return null;
            }
        }

        [GraphQLDescription("Get all statistics for a player")]
        public async Task<IEnumerable<Statistic>?> GetPlayerStatistics([Service] IStatisticRepository repository, int playerId)
        {
            try
            {
                var statistics = await repository.GetMultipleAsync(playerId);
                _logger.LogInformation("Retrieved {Count} statistics for player {PlayerId}", statistics?.Count(), playerId);
                return statistics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get statistics for player {PlayerId}", playerId);
                return null;
            }
        }


        #endregion

        #region Recipes

        [GraphQLDescription("Get a specific recipe for a player")]
        public async Task<Recipe?> GetRecipe([Service] IRecipeRepository repository, int playerId, int typeId)
        {
            try
            {
                var recipe = await repository.FindAsync(playerId, typeId);
                if (recipe == null)
                {
                    _logger.LogInformation("Recipe not found for player {PlayerId} and type {TypeId}", playerId, typeId);
                }
                return recipe;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get recipe for player {PlayerId} and type {TypeId}", playerId, typeId);
                return null;
            }
        }

        [GraphQLDescription("Get all recipes for a player")]
        public async Task<IEnumerable<Recipe>?> GetPlayerRecipes([Service] IRecipeRepository repository, int playerId)
        {
            try
            {
                var recipes = await repository.GetMultipleAsync(playerId);
                _logger.LogInformation("Retrieved {Count} recipes for player {PlayerId}", recipes.Count(), playerId);
                return recipes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get recipes for player {PlayerId}", playerId);
                return null;
            }
        }

        #endregion

        #region Skills
        [GraphQLDescription("Get a specific skill for a player")]
        public async Task<Skill?> GetSkill([Service] ISkillRepository repository, int playerId, int typeId)
        {
            try
            {
                var skill = await repository.FindAsync(playerId, typeId);
                if (skill == null)
                {
                    _logger.LogInformation("Skill not found for player {PlayerId} and type {TypeId}", playerId, typeId);
                }
                return skill;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get skill for player {PlayerId} and type {TypeId}", playerId, typeId);
                return null;
            }
        }

        [GraphQLDescription("Get all skills for a player")]
        public async Task<IEnumerable<Skill>?> GetPlayerSkills([Service] ISkillRepository repository, int playerId)
        {
            try
            {
                var skills = await repository.GetMultipleAsync(playerId);
                _logger.LogInformation("Retrieved {Count} skills for player {PlayerId}", skills?.Count(), playerId);
                return skills;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get skills for player {PlayerId}", playerId);
                return null;
            }
        }
        #endregion

        #region Spell Statistics

        [GraphQLDescription("Get a specific spell statistic for a player")]
        public async Task<SpellStatistic?> GetSpellStatistic([Service] ISpellStatisticRepository repository, int playerId, int typeId)
        {
            try
            {
                var spellStatistic = await repository.FindAsync(playerId, typeId);
                if (spellStatistic == null)
                {
                    _logger.LogInformation("Spell statistic not found for player {PlayerId} and type {TypeId}", playerId, typeId);
                }
                return spellStatistic;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get spell statistic for player {PlayerId} and type {TypeId}", playerId, typeId);
                return null;
            }
        }

        [GraphQLDescription("Get all spell statistics for a player")]
        public async Task<IEnumerable<SpellStatistic>?> GetPlayerSpellStatistics([Service] ISpellStatisticRepository repository, int playerId)
        {
            try
            {
                var spellStatistics = await repository.GetMultipleAsync(playerId);
                _logger.LogInformation("Retrieved {Count} spell statistics for player {PlayerId}", spellStatistics?.Count(), playerId);
                return spellStatistics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get spell statistics for player {PlayerId}", playerId);
                return null;
            }
        }
        #endregion

        #region Elixir

        [GraphQLDescription("Get a specific elixir for a player")]
        public async Task<Elixir?> GetElixir([Service] IElixirRepository repository, int playerId, int elixirId)
        {
            try
            {
                var elixir = await repository.FindAsync(playerId, elixirId);
                if (elixir == null)
                {
                    _logger.LogError("Failed to get elixir for player {PlayerId} and elixir {ElixirId}", playerId, elixirId);
                }
                return elixir;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get elixir for player {PlayerId} and elixir {ElixirId}", playerId, elixirId);
                return null;
            }
        }

        [GraphQLDescription("Get all elixirs for a player")]
        public async Task<IEnumerable<Elixir>?> GetPlayerElixirs([Service] IElixirRepository repository, int playerId)
        {
            try
            {
                var elixirs = await repository.GetPlayerElixirsAsync(playerId);
                _logger.LogInformation("Retrieved {Count} elixirs for player {PlayerId}", elixirs?.Count(), playerId);
                return elixirs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get elixirs for player {PlayerId}", playerId);
                return null;
            }
        }

        #endregion

        #region Game Run

        [GraphQLDescription("Get the active game run for a player")]
        public async Task<GameRun?> GetActiveGameRun([Service] IGameRunRepository repository, int playerId)
        {
            try
            {
                var gameRun = await repository.GetActiveGameRunAsync(playerId);
                if (gameRun == null)
                {
                    _logger.LogInformation("No active game run found for player {PlayerId}", playerId);
                }
                return gameRun;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get active game run for player {PlayerId}", playerId);
                return null;
            }
        }

        #endregion

        #region Leaderboard

        [GraphQLDescription("Get the leaderboard for a specific location")]
        public async Task<IEnumerable<Leaderboard>?> GetLeaderboardByLocation([Service] ILeaderboardRepository repository, int locationId)
        {
            try
            {
                var leaderboard = await repository.GetLeaderboardByLocationAsync(locationId);
                _logger.LogInformation("Retrieved leaderboard for location {LocationId} with {Count} entries", locationId, leaderboard.Count());
                return leaderboard;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get leaderboard for location {LocationId}", locationId);
                return null;
            }
        }

        #endregion

        #region History

        [GraphQLDescription("Get the history entries for a player")]
        public async Task<IEnumerable<History>?> GetPlayerHistory([Service] IHistoryRepository repository, int playerId)
        {
            try
            {
                var history = await repository.GetPlayerHistoryAsync(playerId);
                _logger.LogInformation("Retrieved {Count} history entries for player {PlayerId}", history.Count(), playerId);
                return history;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get history for player {PlayerId}", playerId);
                return null;
            }
        }

        #endregion

        #region Player

        [GraphQLDescription("Get a player by their username")]
        public async Task<Player?> GetPlayerByUsername([Service] IPlayerRepository repository, string username)
        {
            try
            {
                var player = await repository.GetPlayerByUsernameAsync(username);
                if (player == null)
                {
                    _logger.LogInformation("Player not found for username {Username}", username);
                }
                return player;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get player for username {Username}", username);
                return null;
            }
        }


        #endregion

        #region Recipe Shop 

        [GraphQLDescription("Get the recipe shop for a player")]
        public async Task<RecipeShop?> GetRecipeShopByPlayerId([Service] IRecipeShopRepository repository, int playerId)
        {
            try
            {
                var recipeShop = await repository.GetRecipeShopByPlayerIdAsync(playerId);
                if (recipeShop == null)
                {
                    _logger.LogInformation("Recipe shop not found for player {PlayerId}", playerId);
                }
                return recipeShop;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get recipe shop for player {PlayerId}", playerId);
                return null;
            }
        }


        #endregion

    }


}
