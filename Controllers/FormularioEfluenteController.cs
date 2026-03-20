using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CPTM_Backend.Data;
using CPTM_Backend.DTOs;
using CPTM_Backend.Models;

namespace CPTM_Backend.Controllers;

[ApiController]
[Route("api/formularios-efluente")]
[Produces("application/json")]
public class FormularioEfluenteController : ControllerBase
{
    private readonly AppDbContext _db;

    public FormularioEfluenteController(AppDbContext db) => _db = db;

    // ----------------------------------------------------------------
    // GET api/formularios-efluente?pagina=1&tamanho=20
    // ----------------------------------------------------------------
    /// <summary>Lista formulários com paginação, ordenados por data de cadastro (mais recentes primeiro).</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Listar(
        [FromQuery] int pagina  = 1,
        [FromQuery] int tamanho = 20)
    {
        if (pagina < 1 || tamanho < 1 || tamanho > 100)
            return BadRequest("pagina ≥ 1 e tamanho entre 1 e 100.");

        var total = await _db.Formularios.CountAsync();

        var lista = await _db.Formularios
            .AsNoTracking()
            .OrderByDescending(f => f.DtCadastramento)
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .ToListAsync();

        return Ok(new
        {
            Total   = total,
            Pagina  = pagina,
            Tamanho = tamanho,
            Itens   = lista.Select(f => ToDto(f, null))
        });
    }

    // ----------------------------------------------------------------
    // GET api/formularios-efluente/{chavePrimaria}
    // ----------------------------------------------------------------
    /// <summary>Retorna um formulário completo, incluindo suas fotografias.</summary>
    [HttpGet("{chavePrimaria}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(string chavePrimaria)
    {
        var form = await _db.Formularios
            .AsNoTracking()
            .Include(f => f.Fotos)
            .FirstOrDefaultAsync(f => f.ChavePrimariaMa == chavePrimaria);

        if (form is null) return NotFound();

        return Ok(ToDto(form, form.Fotos));
    }

    // ----------------------------------------------------------------
    // POST api/formularios-efluente
    // ----------------------------------------------------------------
    /// <summary>Cria um novo formulário. A ChavePrimariaMa deve ser única.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Criar([FromBody] FormularioEfluenteDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (await _db.Formularios.AnyAsync(f => f.ChavePrimariaMa == dto.ChavePrimariaMa))
            return Conflict($"Já existe um formulário com a chave '{dto.ChavePrimariaMa}'.");

        var model = ToModel(dto);
        AdicionarFotos(model, dto.Fotos);

        _db.Formularios.Add(model);
        await _db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(ObterPorId),
            new { chavePrimaria = model.ChavePrimariaMa },
            ToDto(model, model.Fotos));
    }

    // ----------------------------------------------------------------
    // PUT api/formularios-efluente/{chavePrimaria}
    // ----------------------------------------------------------------
    /// <summary>Atualiza um formulário existente (inclui substituição completa das fotos).</summary>
    [HttpPut("{chavePrimaria}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(string chavePrimaria, [FromBody] FormularioEfluenteDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (chavePrimaria != dto.ChavePrimariaMa)
            return BadRequest("A chave primária do URL e do corpo devem ser iguais.");

        var existente = await _db.Formularios
            .Include(f => f.Fotos)
            .FirstOrDefaultAsync(f => f.ChavePrimariaMa == chavePrimaria);

        if (existente is null) return NotFound();

        AtualizarModel(existente, dto);

        // Substituição completa das fotos
        _db.Fotos.RemoveRange(existente.Fotos);
        existente.Fotos.Clear();
        AdicionarFotos(existente, dto.Fotos);

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ----------------------------------------------------------------
    // DELETE api/formularios-efluente/{chavePrimaria}
    // ----------------------------------------------------------------
    /// <summary>Remove um formulário e suas fotografias (ON DELETE CASCADE).</summary>
    [HttpDelete("{chavePrimaria}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deletar(string chavePrimaria)
    {
        var form = await _db.Formularios
            .FirstOrDefaultAsync(f => f.ChavePrimariaMa == chavePrimaria);

        if (form is null) return NotFound();

        _db.Formularios.Remove(form);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ================================================================
    //  Helpers de mapeamento (privados)
    // ================================================================

    private static FormularioEfluenteDto ToDto(FormularioEfluente m, IEnumerable<FotoEfluente>? fotos)
        => new()
        {
            ChavePrimariaMa         = m.ChavePrimariaMa,
            NrElementoMonit         = m.NrElementoMonit,
            NmElementoMonit         = m.NmElementoMonit,
            NmContratada            = m.NmContratada,
            NrContrato              = m.NrContrato,
            NmLocalEscopo           = m.NmLocalEscopo,
            NmRepresentante         = m.NmRepresentante,
            SgAreaMeioAmbiente      = m.SgAreaMeioAmbiente,
            NmAreaGestoraCptm       = m.NmAreaGestoraCptm,
            CdIdentAreaGestora      = m.CdIdentAreaGestora,
            SgAreaGestoraCptm       = m.SgAreaGestoraCptm,
            NmSupervisoraAmbiental  = m.NmSupervisoraAmbiental,
            NmAutorCadastramento    = m.NmAutorCadastramento,
            NmResponsavelTecnico    = m.NmResponsavelTecnico,
            NrRegistroProfissional  = m.NrRegistroProfissional,
            DsDocRespTecnica        = m.DsDocRespTecnica,
            DsNaturezaPga           = m.DsNaturezaPga,
            DsTipoFormulario        = m.DsTipoFormulario,
            DtEmissaoFormulario     = m.DtEmissaoFormulario,
            NrFormulario            = m.NrFormulario,
            NmAutorFormulario       = m.NmAutorFormulario,
            NmArquivoFdc            = m.NmArquivoFdc,
            CdArquivoFdc            = m.CdArquivoFdc,
            DtCadastramento         = m.DtCadastramento,
            HrCadastramento         = m.HrCadastramento,
            NmMunicipio             = m.NmMunicipio,
            NmLinhaCptm             = m.NmLinhaCptm,
            NmEstacaoCptm           = m.NmEstacaoCptm,
            NrViaLinhaCptm          = m.NrViaLinhaCptm,
            DsTrechoSentidoCptm     = m.DsTrechoSentidoCptm,
            NrKmPoste               = m.NrKmPoste,
            NrLatitude              = m.NrLatitude,
            NrLongitude             = m.NrLongitude,
            DsTipoAtividadeList     = m.DsTipoAtividadeList,
            DsTipoAtividadeNlist    = m.DsTipoAtividadeNlist,
            DsTipoDraList           = m.DsTipoDraList,
            DsTipoDraNlist          = m.DsTipoDraNlist,
            CdIdentificadorDra      = m.CdIdentificadorDra,
            DtValidadeDra           = m.DtValidadeDra,
            DsTipoAtividadeCptm     = m.DsTipoAtividadeCptm,
            NmLocalEdificacao       = m.NmLocalEdificacao,
            DsLocalComplemento      = m.DsLocalComplemento,
            DsOrigemEfluente        = m.DsOrigemEfluente,
            DsFonteGeradora         = m.DsFonteGeradora,
            NrQuantidadeLitros      = m.NrQuantidadeLitros,
            DsTipoDestinacao        = m.DsTipoDestinacao,
            DsTipoVeiculo           = m.DsTipoVeiculo,
            CdPlacaVeiculo          = m.CdPlacaVeiculo,
            CdGuiaRemessa           = m.CdGuiaRemessa,
            NrDistanciaViaM         = m.NrDistanciaViaM,
            DsObservacoesCadastro   = m.DsObservacoesCadastro,
            Fotos = fotos?.Select(f => new FotoDto
            {
                IdFoto       = f.IdFoto,
                NrFoto       = f.NrFoto,
                FotoBase64   = f.BlFoto is not null ? Convert.ToBase64String(f.BlFoto) : null,
                DsOrientacao = f.DsOrientacao
            }).ToList()
        };

    private static FormularioEfluente ToModel(FormularioEfluenteDto dto)
    {
        var m = new FormularioEfluente();
        AtualizarModel(m, dto);
        return m;
    }

    private static void AtualizarModel(FormularioEfluente m, FormularioEfluenteDto dto)
    {
        m.ChavePrimariaMa        = dto.ChavePrimariaMa;
        m.NrElementoMonit        = dto.NrElementoMonit;
        m.NmElementoMonit        = dto.NmElementoMonit;
        m.NmContratada           = dto.NmContratada;
        m.NrContrato             = dto.NrContrato;
        m.NmLocalEscopo          = dto.NmLocalEscopo;
        m.NmRepresentante        = dto.NmRepresentante;
        m.SgAreaMeioAmbiente     = dto.SgAreaMeioAmbiente;
        m.NmAreaGestoraCptm      = dto.NmAreaGestoraCptm;
        m.CdIdentAreaGestora     = dto.CdIdentAreaGestora;
        m.SgAreaGestoraCptm      = dto.SgAreaGestoraCptm;
        m.NmSupervisoraAmbiental = dto.NmSupervisoraAmbiental;
        m.NmAutorCadastramento   = dto.NmAutorCadastramento;
        m.NmResponsavelTecnico   = dto.NmResponsavelTecnico;
        m.NrRegistroProfissional = dto.NrRegistroProfissional;
        m.DsDocRespTecnica       = dto.DsDocRespTecnica;
        m.DsNaturezaPga          = dto.DsNaturezaPga;
        m.DsTipoFormulario       = dto.DsTipoFormulario
                                   ?? "Formulário de Cadastramento - FDC (FDC-EEA.EF)";
        m.DtEmissaoFormulario    = dto.DtEmissaoFormulario;
        m.NrFormulario           = dto.NrFormulario;
        m.NmAutorFormulario      = dto.NmAutorFormulario;
        m.NmArquivoFdc           = dto.NmArquivoFdc;
        m.CdArquivoFdc           = dto.CdArquivoFdc;
        m.DtCadastramento        = dto.DtCadastramento;
        m.HrCadastramento        = dto.HrCadastramento;
        m.NmMunicipio            = dto.NmMunicipio;
        m.NmLinhaCptm            = dto.NmLinhaCptm;
        m.NmEstacaoCptm          = dto.NmEstacaoCptm;
        m.NrViaLinhaCptm         = dto.NrViaLinhaCptm;
        m.DsTrechoSentidoCptm    = dto.DsTrechoSentidoCptm;
        m.NrKmPoste              = dto.NrKmPoste;
        m.NrLatitude             = dto.NrLatitude;
        m.NrLongitude            = dto.NrLongitude;
        m.DsTipoAtividadeList    = dto.DsTipoAtividadeList;
        m.DsTipoAtividadeNlist   = dto.DsTipoAtividadeNlist;
        m.DsTipoDraList          = dto.DsTipoDraList;
        m.DsTipoDraNlist         = dto.DsTipoDraNlist;
        m.CdIdentificadorDra     = dto.CdIdentificadorDra;
        m.DtValidadeDra          = dto.DtValidadeDra;
        m.DsTipoAtividadeCptm    = dto.DsTipoAtividadeCptm;
        m.NmLocalEdificacao      = dto.NmLocalEdificacao;
        m.DsLocalComplemento     = dto.DsLocalComplemento;
        m.DsOrigemEfluente       = dto.DsOrigemEfluente;
        m.DsFonteGeradora        = dto.DsFonteGeradora;
        m.NrQuantidadeLitros     = dto.NrQuantidadeLitros;
        m.DsTipoDestinacao       = dto.DsTipoDestinacao;
        m.DsTipoVeiculo          = dto.DsTipoVeiculo;
        m.CdPlacaVeiculo         = dto.CdPlacaVeiculo;
        m.CdGuiaRemessa          = dto.CdGuiaRemessa;
        m.NrDistanciaViaM        = dto.NrDistanciaViaM;
        m.DsObservacoesCadastro  = dto.DsObservacoesCadastro;
    }

    private static void AdicionarFotos(FormularioEfluente model, List<FotoDto>? fotos)
    {
        if (fotos is null) return;

        foreach (var fDto in fotos)
        {
            model.Fotos.Add(new FotoEfluente
            {
                ChavePrimariaMa = model.ChavePrimariaMa,
                NrFoto          = fDto.NrFoto,
                BlFoto          = fDto.FotoBase64 is not null
                                    ? Convert.FromBase64String(fDto.FotoBase64)
                                    : null,
                DsOrientacao    = fDto.DsOrientacao ?? "Paisagem/Horizontal"
            });
        }
    }
}
