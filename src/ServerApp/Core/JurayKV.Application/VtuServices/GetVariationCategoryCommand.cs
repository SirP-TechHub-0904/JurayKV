using JurayKV.Domain.Aggregates.CategoryVariationAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JurayKV.Domain.Primitives.Enum;
using TanvirArjel.ArgumentChecker;

namespace JurayKV.Application.VtuServices
{
      public sealed class GetVariationCategoryCommand : IRequest<List<CategoryVariation>>
    {
        
    }

    internal class GetVariationCategoryCommandHandler : IRequestHandler<GetVariationCategoryCommand, List<CategoryVariation>>
    {
        private readonly ICategoryVariationRepository _variationRepository;


        public GetVariationCategoryCommandHandler(ICategoryVariationRepository variationRepository)
        {
            _variationRepository = variationRepository;
        }

        public async Task<List<CategoryVariation>> Handle(GetVariationCategoryCommand request, CancellationToken cancellationToken)
        {
            _ = request.ThrowIfNull(nameof(request));

            var result = await _variationRepository.GetAllListAsync();
            return result;
        }
    }
}
