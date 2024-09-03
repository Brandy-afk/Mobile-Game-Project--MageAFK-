

using Backend.Data;
using Backend.Domain.Enums;
using Backend.Domain.Inputs.DTO;
using Backend.Domain.Modals;
using Backend.Domain.Modals.Milestones;
using Backend.Migrations;
using Backend.Payloads;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Mutations
{
    public class Mutation
    {

        #region TypeTables
        [GraphQLDescription("Add entry to a specific table type")]
        public async Task<MutationReturnPayload> AddTableType(
          [Service] GameDbContext context,
          [Service] ITypeRepository repository,
          AbstractTypeInput input
          )
        {
            try
            {
                switch (input.Type)
                {
                    case TableTypeEnum.Currency:
                        await repository.CreateAsync(context.CurrencyTypes, input);
                        break;

                    case TableTypeEnum.Location:
                        await repository.CreateAsync(context.LocationTypes, input);
                        break;

                    case TableTypeEnum.Milestone:
                        await repository.CreateAsync(context.MilestoneTypes, input);
                        break;

                    case TableTypeEnum.PlayerStatistics:
                        await repository.CreateAsync(context.StatisticTypes, input);
                        break;

                    case TableTypeEnum.Recipes:
                        await repository.CreateAsync(context.RecipeTypes, input);
                        break;

                    case TableTypeEnum.Skills:
                        await repository.CreateAsync(context.SkillTypes, input);
                        break;

                    case TableTypeEnum.SpellStatistic:
                        await repository.CreateAsync(context.SpellTypes, input);
                        break;

                    default:
                        throw new ArgumentException($"Unsupported table type: {input.Type}");
                }
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = "Entry added successfully",
                };
            }
            catch (DbUpdateException ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Message = "An update exception occurred!",
                    Error = ex.Message
                };
            }
            catch (ArgumentException ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Message = ex.Message
                };
            }

        }

        [GraphQLDescription("Delete a entry to a specific table type")]
        public async Task<MutationReturnPayload> DeleteTableType(
            TableTypeEnum tableType,
            int ID,
            [Service] GameDbContext context,
            [Service] ITypeRepository repository)
        {
            try
            {
                switch (tableType)
                {
                    case TableTypeEnum.Currency:
                        await repository.DeleteAsync(context.CurrencyTypes, ID);
                        break;
                    case TableTypeEnum.Location:
                        await repository.DeleteAsync(context.LocationTypes, ID);
                        break;
                    case TableTypeEnum.Milestone:
                        await repository.DeleteAsync(context.MilestoneTypes, ID);
                        break;
                    case TableTypeEnum.PlayerStatistics:
                        await repository.DeleteAsync(context.StatisticTypes, ID);
                        break;
                    case TableTypeEnum.Recipes:
                        await repository.DeleteAsync(context.RecipeTypes, ID);
                        break;
                    case TableTypeEnum.Skills:
                        await repository.DeleteAsync(context.SkillTypes, ID);
                        break;
                    case TableTypeEnum.SpellStatistic:
                        await repository.DeleteAsync(context.SpellTypes, ID);
                        break;


                    default:
                        throw new ArgumentException($"Unsupported table type: {tableType}");
                }
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = "Entry added successfully",
                };
            }
            catch (ArgumentException ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Message = "An update exception occurred!",
                    Error = ex.Message
                };
            }
        }
        #endregion

        #region Currency

        [GraphQLDescription("Create multiple currency entries for a player.")]
        public async Task<MutationReturnPayload> CreateCurrencies([Service] ICurrencyRepository repository, int playerId, IEnumerable<int> typeIds)
        {
            try
            {
                var currencies = await repository.CreateAsync(playerId, typeIds);
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully created {currencies.Count()} currencies for player {playerId}"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to create currencies: {ex.Message}"
                };
            }
        }

        [GraphQLDescription("Update currency entries for a player.")]
        public async Task<MutationReturnPayload> UpdateCurrencies([Service] ICurrencyRepository repository, int playerId, Dictionary<int, int> valuePairs)
        {
            try
            {
                var updatedCurrencies = await repository.UpdateAsync(playerId, valuePairs);
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully updated {updatedCurrencies.Count()} currencies for player {playerId}"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to update currencies: {ex.Message}"
                };
            }
        }

        [GraphQLDescription("Delete a currency entrie for a player. FOR TESTING ONLY")]
        public async Task<MutationReturnPayload> DeleteCurrency([Service] ICurrencyRepository repository, int playerId, int typeId)
        {
            try
            {
                await repository.DeleteAsync(playerId, typeId);
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully deleted currency type {typeId} for player {playerId}"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to delete currency: {ex.Message}"
                };
            }
        }

        #endregion

        #region Location


        [GraphQLDescription("Create new location data for a player")]
        public async Task<MutationReturnPayload> CreateLocations([Service] ILocationRepository repository, int playerId, IEnumerable<int> typeIds)
        {
            try
            {
                var locations = await repository.CreateAsync(playerId, typeIds);
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully created {locations.Count()} locations for player {playerId}"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to create locations: {ex.Message}"
                };
            }
        }

        [GraphQLDescription("Update location data for a player")]
        public async Task<MutationReturnPayload> UpdateLocations([Service] ILocationRepository repository, int playerId, Dictionary<int, int> bestWavePairs)
        {
            try
            {
                var updatedLocations = await repository.UpdateAsync(playerId, bestWavePairs);
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully updated {updatedLocations.Count()} locations for player {playerId}"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to update locations: {ex.Message}"
                };
            }
        }

        [GraphQLDescription("Delete a location for a player")]
        public async Task<MutationReturnPayload> DeleteLocation([Service] ILocationRepository repository, int playerId, int typeId)
        {
            try
            {
                await repository.DeleteAsync(playerId, typeId);
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully deleted location type {typeId} for player {playerId}"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to delete location: {ex.Message}"
                };
            }
        }


        #endregion

        #region Milestones

        [GraphQLDescription("Create new milestones for a player")]
        public async Task<MutationReturnPayload> CreateMilestones([Service] IMilestoneRepository repository, int playerId, IEnumerable<int> typeIds)
        {
            try
            {
                var milestones = await repository.CreateAsync(playerId, typeIds);
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully created {milestones.Count()} milestones for player {playerId}"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to create milestones: {ex.Message}"
                };
            }
        }

        [GraphQLDescription("Update milestones for a player")]
        public async Task<MutationReturnPayload> UpdateMilestones([Service] IMilestoneRepository repository, int playerId, Dictionary<int, MilestoneUpdateInput> updates)
        {
            try
            {
                var updatePairs = updates.ToDictionary(
                    kvp => kvp.Key,
                    kvp => (kvp.Value.Value, kvp.Value.Rank, kvp.Value.RewardPoolSize)
                );
                var updatedMilestones = await repository.UpdateAsync(playerId, updatePairs);
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully updated {updatedMilestones.Count()} milestones for player {playerId}"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to update milestones: {ex.Message}"
                };
            }
        }

        [GraphQLDescription("Delete a milestone for a player")]
        public async Task<MutationReturnPayload> DeleteMilestone([Service] IMilestoneRepository repository, int playerId, int typeId)
        {
            try
            {
                await repository.DeleteAsync(playerId, typeId);
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully deleted milestone type {typeId} for player {playerId}"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to delete milestone: {ex.Message}"
                };
            }
        }


        #endregion

        #region Statistics

        [GraphQLDescription("Create new statistics for a player")]
        public async Task<MutationReturnPayload> CreateStatistics([Service] IStatisticRepository repository, int playerId, IEnumerable<int> typeIds)
        {
            try
            {
                var statistics = await repository.CreateAsync(playerId, typeIds);
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully created {statistics.Count()} statistics for player {playerId}"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to create statistics: {ex.Message}"
                };
            }
        }

        [GraphQLDescription("Update statistics for a player")]
        public async Task<MutationReturnPayload> UpdateStatistics([Service] IStatisticRepository repository, int playerId, Dictionary<int, float> updates)
        {
            try
            {
                var updatedStatistics = await repository.UpdateAsync(playerId, updates);
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully updated {updatedStatistics.Count()} statistics for player {playerId}"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to update statistics: {ex.Message}"
                };
            }
        }

        [GraphQLDescription("Delete a statistic for a player")]
        public async Task<MutationReturnPayload> DeleteStatistic([Service] IStatisticRepository repository, int playerId, int typeId)
        {
            try
            {
                await repository.DeleteAsync(playerId, typeId);
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully deleted statistic type {typeId} for player {playerId}"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to delete statistic: {ex.Message}"
                };
            }
        }


        #endregion

        #region Recipes

        [GraphQLDescription("Create new recipes for a player")]
        public async Task<MutationReturnPayload> CreateRecipes([Service] IRecipeRepository repository, int playerId, IEnumerable<int> typeIds)
        {
            try
            {
                var recipes = await repository.CreateAsync(playerId, typeIds);
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully created {recipes.Count()} recipes for player {playerId}"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to create recipes: {ex.Message}"
                };
            }
        }

        [GraphQLDescription("Update recipes for a player")]
        public async Task<MutationReturnPayload> UpdateRecipes([Service] IRecipeRepository repository, int playerId, Dictionary<int, bool> updates)
        {
            try
            {
                var updatedRecipes = await repository.UpdateAsync(playerId, updates);
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully updated {updatedRecipes.Count()} recipes for player {playerId}"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to update recipes: {ex.Message}"
                };
            }
        }

        [GraphQLDescription("Delete a recipe for a player")]
        public async Task<MutationReturnPayload> DeleteRecipe([Service] IRecipeRepository repository, int playerId, int typeId)
        {
            try
            {
                await repository.DeleteAsync(playerId, typeId);
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully deleted recipe type {typeId} for player {playerId}"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to delete recipe: {ex.Message}"
                };
            }
        }


        #endregion

        #region Skills

        [GraphQLDescription("Create new skills for a player")]
        public async Task<MutationReturnPayload> CreateSkills([Service] ISkillRepository repository, int playerId, IEnumerable<int> typeIds)
        {
            try
            {
                var skills = await repository.CreateAsync(playerId, typeIds);
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully created {skills.Count()} skills for player {playerId}"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to create skills: {ex.Message}"
                };
            }
        }

        [GraphQLDescription("Update skills for a player")]
        public async Task<MutationReturnPayload> UpdateSkills([Service] ISkillRepository repository, int playerId, Dictionary<int, int> updates)
        {
            try
            {
                var updatedSkills = await repository.UpdateAsync(playerId, updates);
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully updated {updatedSkills.Count()} skills for player {playerId}"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to update skills: {ex.Message}"
                };
            }
        }

        [GraphQLDescription("Delete a skill for a player")]
        public async Task<MutationReturnPayload> DeleteSkill([Service] ISkillRepository repository, int playerId, int typeId)
        {
            try
            {
                await repository.DeleteAsync(playerId, typeId);
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully deleted skill type {typeId} for player {playerId}"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to delete skill: {ex.Message}"
                };
            }
        }

        #endregion

        #region Spell Statistics

        [GraphQLDescription("Create new spell statistics for a player")]
        public async Task<MutationReturnPayload> CreateSpellStatistics([Service] ISpellStatisticRepository repository, int playerId, IEnumerable<int> typeIds)
        {
            try
            {
                var spellStatistics = await repository.CreateAsync(playerId, typeIds);
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully created {spellStatistics.Count()} spell statistics for player {playerId}"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to create spell statistics: {ex.Message}"
                };
            }
        }

        [GraphQLDescription("Update spell statistics for a player")]
        public async Task<MutationReturnPayload> UpdateSpellStatistics([Service] ISpellStatisticRepository repository, int playerId, Dictionary<int, SpellStatisticUpdateInput> updates)
        {
            try
            {
                var updatedSpellStatistics = await repository.UpdateAsync(playerId, updates);
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully updated {updatedSpellStatistics.Count()} spell statistics for player {playerId}"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to update spell statistics: {ex.Message}"
                };
            }
        }

        [GraphQLDescription("Delete a spell statistic for a player")]
        public async Task<MutationReturnPayload> DeleteSpellStatistic([Service] ISpellStatisticRepository repository, int playerId, int typeId)
        {
            try
            {
                await repository.DeleteAsync(playerId, typeId);
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully deleted spell statistic type {typeId} for player {playerId}"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to delete spell statistic: {ex.Message}"
                };
            }
        }

        #endregion

        #region Elixir

        [GraphQLDescription("Create new elixirs for a player")]
        public async Task<MutationReturnPayload> CreateElixirs([Service] IElixirRepository repository, int playerId, IEnumerable<CreateElixirInput> elixirInputs)
        {
            try
            {
                var elixirs = elixirInputs.Select(input => new Elixir
                {
                    PlayerID = playerId,
                    ElixirID = input.ElixirID,
                    Purchased = false,
                    Cost = input.Cost,
                    Value = input.Value
                });

                var createdElixirs = await repository.CreateAsync(playerId, elixirs);
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully created {createdElixirs.Count()} elixirs for player {playerId}"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to create elixirs: {ex.Message}"
                };
            }
        }

        [GraphQLDescription("Update an elixir's purchase status for a player")]
        public async Task<MutationReturnPayload> UpdateElixirPurchaseStatus([Service] IElixirRepository repository, int playerId, int elixirId, bool purchased)
        {
            try
            {
                var updatedElixir = await repository.UpdateAsync(playerId, elixirId, purchased);
                if (updatedElixir != null)
                {
                    return new MutationReturnPayload
                    {
                        Success = true,
                        Message = $"Successfully updated elixir {elixirId} purchase status for player {playerId}"
                    };
                }
                else
                {
                    return new MutationReturnPayload
                    {
                        Success = false,
                        Error = $"Elixir {elixirId} not found for player {playerId}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to update elixir: {ex.Message}"
                };
            }
        }

        [GraphQLDescription("Delete an elixir for a player")]
        public async Task<MutationReturnPayload> DeleteElixir([Service] IElixirRepository repository, int playerId, int elixirId)
        {
            try
            {
                await repository.DeleteAsync(playerId, elixirId);
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully deleted elixir {elixirId} for player {playerId}"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to delete elixir: {ex.Message}"
                };
            }
        }


        #endregion

        #region Game Runs

        [GraphQLDescription("Create a new game run for a player")]
        public async Task<MutationReturnPayload> CreateGameRun([Service] IGameRunRepository repository, int playerId, string gameState)
        {
            try
            {
                var gameRun = await repository.CreateGameRunAsync(playerId, gameState);
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully created game run {gameRun.GameRunID} for player {playerId}"
                };
            }
            catch (Exception ex)
            {

                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to create game run: {ex.Message}"
                };
            }
        }

        [GraphQLDescription("Complete the active game run for a player")]
        public async Task<MutationReturnPayload> CompleteGameRun([Service] IGameRunRepository repository, int playerId)
        {
            try
            {
                var completedGameRun = await repository.CompleteGameRunAsync(playerId);
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully completed game run {completedGameRun?.GameRunID} for player {playerId}"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to complete game run: {ex.Message}"
                };
            }
        }

        [GraphQLDescription("Delete completed game runs older than 7 days")]
        public async Task<MutationReturnPayload> DeleteOldCompletedGameRuns([Service] IGameRunRepository repository)
        {
            try
            {
                var deletedCount = await repository.DeleteOldCompletedGameRunsAsync();
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully deleted {deletedCount} old completed game runs"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to delete old completed game runs: {ex.Message}"
                };
            }
        }

        #endregion

        #region History

        [GraphQLDescription("Create a new history entry for a player")]
        public async Task<MutationReturnPayload> CreateHistoryEntry([Service] IHistoryRepository repository, HistoryInput input)
        {
            try
            {
                var historyEntry = new History
                {
                    PlayerID = input.PlayerID,
                    LocationID = input.LocationID,
                    BestWave = input.BestWave,
                    Damage = input.Damage,
                    Level = input.Level,
                    Wave = input.Wave,
                    Metrics = input.Metrics,
                    Mob = input.Mob,
                    Spell = input.Spell
                };

                var createdEntry = await repository.CreateHistoryEntryAsync(historyEntry);

                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully created history entry for player {input.PlayerID}"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to create history entry: {ex.Message}"
                };
            }
        }

        #endregion

        #region Player

        [GraphQLDescription("Create a new player")]
        public async Task<MutationReturnPayload> CreatePlayer([Service] IPlayerRepository repository, string username)
        {
            try
            {
                var existingPlayer = await repository.GetPlayerByUsernameAsync(username);
                if (existingPlayer != null)
                {
                    return new MutationReturnPayload
                    {
                        Success = false,
                        Error = $"A player with username {username} already exists"
                    };
                }

                var player = await repository.CreatePlayerAsync(username);
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully created player with username {username} and ID {player.ID}"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to create player: {ex.Message}"
                };
            }
        }

        [GraphQLDescription("Delete a player based on their ID")]
        public async Task<MutationReturnPayload> DeletePlayer([Service] IPlayerRepository repository, int id)
        {
            try
            {
                var deletedObject = await repository.DeletePlayerAsync(id);

                return new MutationReturnPayload
                {
                    Success = true,
                    Error = $"Deleted player with id : {id} - Username : {deletedObject.Username}"
                };

            }
            catch(Exception ex) 
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to delete player: {ex.Message}"
                };
            }

        }

        #endregion

        #region Recipe Shop 

        [GraphQLDescription("Create or replace the recipe shop for a player")]
        public async Task<MutationReturnPayload> CreateRecipeShop([Service] IRecipeShopRepository repository, int playerId, List<int> recipeIds)
        {
            try
            {
                var recipeShop = await repository.CreateRecipeShopAsync(playerId, recipeIds);
                return new MutationReturnPayload
                {
                    Success = true,
                    Message = $"Successfully created/replaced recipe shop for player {playerId} with {recipeIds.Count} recipes"
                };
            }
            catch (Exception ex)
            {
                return new MutationReturnPayload
                {
                    Success = false,
                    Error = $"Failed to create/replace recipe shop: {ex.Message}"
                };
            }
        }

        #endregion
    }
}
