using Moq;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Tests.Mocks
{
    public class IBaseDAOMock<TEntity> : Mock<IBaseDAO<TEntity>>
        where TEntity : class
    {
        public IBaseDAOMock(MockBehavior mockBehavior = MockBehavior.Strict) : base(mockBehavior)
        {
        }

        public static IBaseDAOMock<TEntity> Create()
        {
            IBaseDAOMock<TEntity> baseDAOMock = new IBaseDAOMock<TEntity>();

            return baseDAOMock;
        }

        public IBaseDAOMock<TEntity> Add_Success()
        {
            Setup(x => x.Add(It.IsAny<TEntity>()))
                .Returns(Task.FromResult(true));

            return this;
        }

        public IBaseDAOMock<TEntity> Add_Failure()
        {
            Setup(x => x.Add(It.IsAny<TEntity>()))
                .Returns(Task.FromResult(false));

            return this;
        }

        public IBaseDAOMock<TEntity> Exists_Success()
        {
            Setup(x => x.Exist(It.IsAny<IBaseSpecification<TEntity, It.IsAnyType>>()))
                .Returns(Task.FromResult(true));

            return this;
        }

        public IBaseDAOMock<TEntity> Exists_Failure()
        {
            Setup(x => x.Exist(It.IsAny<IBaseSpecification<TEntity, It.IsAnyType>>()))
                .Returns(Task.FromResult(false));

            return this;
        }
    }
}
