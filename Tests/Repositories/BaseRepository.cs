using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApi.Exceptions;
using WebApi.Models;
using WebApi.Repositories;

namespace WebApi.Tests.Repositories
{
    public class FacilityRepository : BaseRepository<Facility>
    {
        public FacilityRepository(ApplicationDbContext dbContext,
            ILogger<BaseRepository<Facility>> logger)
            : base(dbContext, logger)
        {
        }
    }

    public class BaseRepositoryTests
    {
        private DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private ApplicationDbContext _dbContext;
        private BaseRepository<Facility> _repository;

        [SetUp]
        public void SetUp()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Create a new in-memory database for each test
                .Options;
            _dbContext = new ApplicationDbContext(_dbContextOptions);
            _repository = new FacilityRepository(_dbContext,
                Mock.Of<ILogger<BaseRepository<Facility>>>());
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        [Test]
        public async Task GetById_ShouldReturnEntity_WhenEntityExists()
        {
            var entity = new Facility { Id = 1, Name = "Test Facility" };
            await _dbContext.Set<Facility>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetById(1);
            Assert.That(result, Is.EqualTo(entity));
        }

        [Test]
        public void GetById_ShouldThrowRepositoryException_WhenEntityDoesNotExist()
        {
            Assert.ThrowsAsync<RepositoryException>(() => _repository.GetById(1));
        }

        [Test]
        public async Task Add_ShouldAddEntity_WhenSuccessful()
        {
            var entity = new Facility { Id = 1, Name = "Test Facility" };

            var result = await _repository.Add(entity);
            Assert.That(result, Is.EqualTo(entity));

            var addedEntity = await _dbContext.Set<Facility>().FindAsync(1);
            Assert.That(addedEntity, Is.EqualTo(entity));
        }

        [Test]
        public void Add_ShouldThrowRepositoryException_WhenDbUpdateExceptionOccurs()
        {
            var dbContextMock = new Mock<ApplicationDbContext>(_dbContextOptions);
            dbContextMock.Setup(m => m.Set<Facility>().AddAsync(It.IsAny<Facility>(), default))
                .ThrowsAsync(new DbUpdateException());

            var repository = new FacilityRepository(dbContextMock.Object,
                Mock.Of<ILogger<BaseRepository<Facility>>>());
            var entity = new Facility { Id = 1, Name = "Test Facility" };

            Assert.ThrowsAsync<RepositoryException>(() => repository.Add(entity));
        }

        [Test]
        public async Task Update_ShouldUpdateEntity_WhenSuccessful()
        {
            var entity = new Facility { Id = 1, Name = "Test Facility" };
            await _dbContext.Set<Facility>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            entity.Name = "Updated Facility";
            entity.UpdatedOn = DateTime.UtcNow;
            var result = await _repository.Update(entity);

            Assert.That(result, Is.EqualTo(entity));
            var updatedEntity = await _dbContext.Set<Facility>().FindAsync(1);
            Assert.That(updatedEntity.Name, Is.EqualTo(entity.Name));
            Assert.That(updatedEntity.UpdatedOn, Is.EqualTo(entity.UpdatedOn));
        }

        [Test]
        public async Task Delete_ShouldRemoveEntity_WhenSuccessful()
        {
            var entity = new Facility { Id = 1, Name = "Test Facility" };
            await _dbContext.Set<Facility>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            await _repository.Delete(1);

            var deletedEntity = await _dbContext.Set<Facility>().FindAsync(1);
            Assert.That(deletedEntity, Is.Null);
        }

        [Test]
        public void Delete_ShouldThrowRepositoryException_WhenDbUpdateExceptionOccurs()
        {
            var dbContextMock = new Mock<ApplicationDbContext>(_dbContextOptions);
            dbContextMock.Setup(m => m.Set<Facility>().FindAsync(It.IsAny<int>()))
                .ReturnsAsync(new Facility { Id = 1, Name = "Test Facility" });
            dbContextMock.Setup(m => m.SaveChangesAsync(default))
                .ThrowsAsync(new DbUpdateException());

            var repository = new FacilityRepository(dbContextMock.Object,
                Mock.Of<ILogger<BaseRepository<Facility>>>());

            Assert.ThrowsAsync<RepositoryException>(() => repository.Delete(1));
        }
    }
}
