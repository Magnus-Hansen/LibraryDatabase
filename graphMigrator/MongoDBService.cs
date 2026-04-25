using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphMigrator
{
    public class MigrateToMongo
    {
        private readonly MongoDataSource _mongoDataSource;
        private readonly MySqlDataFetcher _mysqlFetcher;

        public MigrateToMongo(
            MongoDataSource mongoDataSource,
            MySqlDataFetcher mysqlFetcher)
        {
            _mongoDataSource = mongoDataSource;
            _mysqlFetcher = mysqlFetcher;
        }

        public async Task ExecuteAsync()
        {
            var data = await _mysqlFetcher.FetchAsync();

            if (data == null)
            {
                Console.WriteLine("No data fetched from MySQL, exiting...");
                return;
            }

            var (airlines, airports, flights, passengers, bookings) = data;

            try
            {
                var repos = await InitializeMongoDataSourceAsync();

                if (repos == null)
                {
                    Console.WriteLine("Error in MongoDB DataSource, exiting...");
                    return;
                }

                await repos.MongoAirlineRepository.InsertManyAsync(airlines);
                Console.WriteLine("Airlines data copied to MongoDB!");

                await repos.MongoAirportRepository.InsertManyAsync(airports);
                Console.WriteLine("Airport data copied to MongoDB!");

                await repos.MongoFlightRepository.InsertManyAsync(flights);
                Console.WriteLine("Flights data copied to MongoDB!");

                await repos.MongoPassengerRepository.InsertManyAsync(passengers);
                Console.WriteLine("Passengers data copied to MongoDB!");

                await repos.MongoBookingRepository.InsertManyAsync(bookings);
                Console.WriteLine("Bookings data copied to MongoDB!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                await _mongoDataSource.DisposeAsync();
            }
        }
    }
}
