﻿using AutoMapper;
using AwesomeDevEvents.API.Entities;
using AwesomeDevEvents.API.Models;
using AwesomeDevEvents.API.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AwesomeDevEvents.API.Controllers
{
    [Route("api/Dev-Events")]
    [ApiController]
    public class DevEventsController : ControllerBase
    {
        private readonly DevEventDbContext _context;
        private readonly IMapper _mapper;
        

        public DevEventsController(DevEventDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Obter todos os Eventos
        /// </summary>
        /// <returns>Coleção/Lista de eventos</returns>
        /// <response code="200">Sucesso</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            var DevEvents = _context.DevEvents.Where(d => !d.IsDeleted).ToList();

            var viewModel = _mapper.Map<List<DevEventViewModel>>(DevEvents);

            return Ok(viewModel);
        }

        /// <summary>
        /// Obter um evento buscando pelo um ID
        /// </summary>
        /// <param name="id">Identificador de um evento</param>
        /// <returns>dados de um determinado evento</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="404">Não encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(Guid id)
        {
            var devEvent = _context.DevEvents.Include(de => de.Speakers).SingleOrDefault(d => d.Id == id);
            if (devEvent == null)
            {
                return NotFound();
            }
            var viewModel = _mapper.Map<DevEventViewModel>(devEvent);

            return Ok(viewModel);
        }

        /// <summary>
        /// Cadastrar um novo evento
        /// </summary>
        /// <remarks>
        /// {"title":"string","description":"string","startDate":"2023-02-27T17:59:14.141Z","endDate":"2023-02-27T17:59:14.141Z"}
        /// </remarks>
        /// <param name="input">Dados do Evento</param>
        /// <returns>Objeto Criado</returns>
        /// <response code="201">Sucesso-Criado</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult Post(DevEventInputModel input)
        {
            var devEvent = _mapper.Map<DevEvent>(input);

            _context.DevEvents.Add(devEvent);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = devEvent.Id }, devEvent);

        }

        /// <summary>
        /// Atualizar informações de um evento 
        /// </summary>
        /// <remarks>
        /// {"title":"string","description":"string","startDate":"2023-02-27T17:59:14.141Z","endDate":"2023-02-27T17:59:14.141Z"}
        /// </remarks>
        /// <param name="id">Identificador do evento</param>
        /// <param name="input">Dados do Evento</param>
        /// <returns>Nada</returns>
        /// <response code="204">Sucesso-Atualizado</response>
        /// <response code="404">Não encontrado</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Update(Guid id, DevEventInputModel input)
        {
            var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == id);
            if (devEvent == null)
            {
                return NotFound();
            }

            devEvent.Update(input.Title, input.Description, input.StartDate, input.EndDate);

            _context.DevEvents.Update(devEvent);
            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Excluir um Evento
        /// </summary>
        /// <param name="id">Identificador do evento</param>
        /// <returns>Nada</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Delete(Guid id)
        {
            var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == id);
            if (devEvent == null)
            {
                return NotFound();
            }

            devEvent.Delete();

            _context.SaveChanges();

            return NoContent();
        }


        /// <summary>
        /// Cadastro do Palestrante
        /// </summary>
        /// <remarks>
        /// {"name":"string","talkTitle":"string","talkDescription":"string","linkedInProfile":"string"}
        /// </remarks>
        /// <param name="id">Identificador de um Palestrante</param>
        /// <param name="input">Dados do Palestrante</param>
        /// <returns>Nada</returns>
        /// <response code="204">Sucesso-Atualizado</response>
        /// <response code="404">Não encontrado</response> 
        /// 
        [HttpPut("{id}/speakers")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult PostSpearks(Guid id, DevEventSpeakerInputModel input)
        {
            var spearker = _mapper.Map<DevEventSpeaker>(input);

            spearker.DevEventId = id;

            var devEvent = _context.DevEvents.Any(d => d.Id == id);

            if (!devEvent)
            {
                return NotFound();
            }

            _context.DevEventsSpeakers.Add(spearker);
            _context.SaveChanges();

            return NoContent();
        }

    }
}
