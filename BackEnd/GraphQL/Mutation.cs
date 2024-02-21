﻿using AutoMapper;
using BackEnd.DTOs;
using BackEnd.Entities;
using BackEnd.Repositories;
using HotChocolate.Authorization;

namespace BackEnd.GraphQL
{
    public class Mutation
    {
        [Serial]
        [Authorize(Policy = "isAdmin")]
        public async Task<GeneroDTO> InsertarGenero([Service] IRepositorioGeneros repositorioGeneros,
            [Service] IMapper mapper, CrearGeneroDTO crearGeneroDTO)
        {
            var genero = mapper.Map<Genero>(crearGeneroDTO);
            await repositorioGeneros.Crear(genero);
            var generoDTO = mapper.Map<GeneroDTO>(genero);
            return generoDTO;
        }

        [Serial]
        [Authorize(Policy = "isAdmin")]
        public async Task<GeneroDTO?> ActualizarGenero([Service] IRepositorioGeneros repositorioGeneros,
            [Service] IMapper mapper, ActualizarGeneroDTO actualizarGeneroDTO)
        {
            var generoExiste = await repositorioGeneros.Existe(actualizarGeneroDTO.Id);

            if (!generoExiste)
            {
                return null;
            }

            var genero = mapper.Map<Genero>(actualizarGeneroDTO);
            await repositorioGeneros.Actualizar(genero);
            var generoDTO = mapper.Map<GeneroDTO>(genero);
            return generoDTO;
        }

        [Serial]
        [Authorize(Policy = "isAdmin")]
        public async Task<bool> BorrarGenero([Service] IRepositorioGeneros repositorioGeneros,
           int id)
        {
            var generoExiste = await repositorioGeneros.Existe(id);

            if (!generoExiste)
            {
                return false;
            }

            await repositorioGeneros.Borrar(id);
            return true;
        }
    }
}
