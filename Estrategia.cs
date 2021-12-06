
using System;
using System.Collections;
using System.Collections.Generic;

namespace DeepSpace
{

	class Estrategia
	{
		// CONSULTA 1: Calcula y retorna un texto con la distancia del camino que existe entre el planeta del Bot y la raíz. //
		
		public String Consulta1( ArbolGeneral<Planeta> arbol)
		{	int nivel = 0;
			ArbolGeneral<Planeta> bot = FindBot(arbol);
			nivel = arbol.nivel(bot.getDatoRaiz());

			return "La distancia del Bot a la raíz es de: " + Convert.ToString(nivel);
		}

		// CONSULTA 2: Retorna un texto con el listado de los planetas ubicados en todos los descendientes del nodo que contiene al planeta del Bot //

		public String Consulta2( ArbolGeneral<Planeta> arbol )
		{	string descendientes = " ";
			
			ArbolGeneral<Planeta> bot = FindBot(arbol);
			if (!bot.esHoja())
				descendientes = RecorridoPoblacion(bot, descendientes);
			else
				descendientes = "El Bot no tiene descendientes";
	
			return descendientes;
		}
		
		// CONSULTA 3: Calcula y retorna en un texto la población total y promedio por cada nivel del árbol.//
		
		public String Consulta3( ArbolGeneral<Planeta> arbol)
		{	Cola<ArbolGeneral<Planeta>> c = new Cola<ArbolGeneral<Planeta>>();
			ArbolGeneral<Planeta> arbolAux;
			string info = "";
			int nivel = 0;
			uint totalPoblacion = 0;
			uint promPoblacion = 0;
			int cantNodos = 0;

			c.encolar((ArbolGeneral<Planeta>)arbol);
			c.encolar(null);
			
			while(!c.esVacia())
			{	arbolAux = c.desencolar();
				
				if(arbolAux == null){
					if(!c.esVacia())
					{	
						info = info + "La población total del nivel "+nivel + " es: "+totalPoblacion
								+ ", la población promedio es: "+promPoblacion +"\n";
						nivel++;
						totalPoblacion = 0;
						cantNodos = 0;
						c.encolar(null);
					}
					else
					{
						info = info + "La población total del nivel "+nivel + " es: "+totalPoblacion
								+ ", la población promedio es: "+promPoblacion +"\n";
					}
				}
				else
				{	totalPoblacion += Convert.ToUInt32(arbolAux.getDatoRaiz().Poblacion());
					cantNodos++;
					promPoblacion = totalPoblacion/Convert.ToUInt32(cantNodos);
					foreach (ArbolGeneral<Planeta> ar in arbolAux.getHijos())
						c.encolar(ar);
				}
			}
			
			return info;
		}
		
		// CALCULAR MOVIMIENTO: Este método calcula y retorna el movimiento apropiado según el estado del juego. //
		
		public Movimiento CalcularMovimiento(ArbolGeneral<Planeta> arbol)
		{	ArbolGeneral<Planeta> bot = FindBot(arbol);
			ArbolGeneral<Planeta> jugador = FindJugador(arbol);
			
			List<ArbolGeneral<Planeta>> caminoHaciaJugador = new List<ArbolGeneral<Planeta>>();
			List<ArbolGeneral<Planeta>> caminoDesdeBot = new List<ArbolGeneral<Planeta>>();
			
			
			if(!bot.esHoja() && arbol.esDescendiente(bot, jugador))
			{	if(arbol.esHijo(jugador, bot))
					return new Movimiento(bot.getDatoRaiz(), jugador.getDatoRaiz());
				
				caminoHaciaJugador = CaminoAlJugador(jugador, bot, caminoHaciaJugador, bot);
				
			
				foreach(ArbolGeneral<Planeta> nodo in caminoHaciaJugador)
				{	if(nodo.getDatoRaiz().EsPlanetaNeutral()||nodo.getDatoRaiz().EsPlanetaDelJugador()) //ataca a planeta vecino
						return new Movimiento(bot.getDatoRaiz(), nodo.getDatoRaiz());
					
					//si el planeta vecino ya es de la IA, y tiene 2 veces menos población que el Bot... 
					if(nodo.getDatoRaiz().EsPlanetaDeLaIA() && nodo.getDatoRaiz().Poblacion()<bot.getDatoRaiz().Poblacion()*2) 
						return new Movimiento(bot.getDatoRaiz(), nodo.getDatoRaiz()); //..retorna un movimiento de recarga entre el bot y el nodo vecino
					
					//sino, el nodo pasa a ser el bot
					bot = nodo;
				}
			}
			
			
			else
			{	caminoDesdeBot = CaminoDesdeBot(bot, arbol, caminoDesdeBot, arbol);
				
				foreach(ArbolGeneral<Planeta> nodo in caminoDesdeBot)
					return new Movimiento(bot.getDatoRaiz(), nodo.getDatoRaiz());	
			}
				
			return new  Movimiento(null, null);
			
		}
		
			
	//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>   MÉTODOS PRIVADOS  <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<//
		
		// Recorre los nodos de un ArbolGeneral<Planeta> y retorna un string con la Población de cada uno. //
		private string RecorridoPoblacion (ArbolGeneral<Planeta> bot, string lista) //utilizado en Consulta2
		{	string listado = lista;
			Cola<ArbolGeneral<Planeta>> c = new Cola<ArbolGeneral<Planeta>>();
			ArbolGeneral<Planeta> arbolAux;
			
			c.encolar(bot);
			while(!c.esVacia())
			{	arbolAux = c.desencolar();
				if(arbolAux == bot)
					listado = "Población por cada descendiente del Bot:";
				else
					listado = listado + " [" + Convert.ToString(arbolAux.getDatoRaiz().Poblacion()) + "]";
			
			foreach(ArbolGeneral<Planeta> hijo in arbolAux.getHijos())
					c.encolar(hijo);
			}
			
			return listado;
		}
		
		// Recorre los nodos de un ArbolGeneral<Planeta> y retorna el perteneciente al Bot. //
		private ArbolGeneral<Planeta> FindBot( ArbolGeneral<Planeta> arbol)   //Utilizado en Consulta1, Consulta2 y CalcularMovimiento
		{ 	Cola<ArbolGeneral<Planeta>> c = new Cola<ArbolGeneral<Planeta>>();
			ArbolGeneral<Planeta> arbolAux;
			ArbolGeneral<Planeta> bot = new ArbolGeneral<Planeta> (null);
			
			c.encolar(arbol);
			while(!c.esVacia()){
				arbolAux = c.desencolar();
				
				if(arbolAux.getDatoRaiz().EsPlanetaDeLaIA())
				{	bot = arbolAux;
					break;
				}
				foreach(ArbolGeneral<Planeta> hijo in arbolAux.getHijos())
					c.encolar(hijo);
			}
			
			return bot;
        }
		
		// Recorre los nodos de un ArbolGeneral<Planeta> y retorna el perteneciente al Jugador-Humano. //
		private ArbolGeneral<Planeta> FindJugador (ArbolGeneral<Planeta> arbol) //Utilizado en CalcularMovimento
		{	Cola<ArbolGeneral<Planeta>> c = new Cola<ArbolGeneral<Planeta>>();
			ArbolGeneral<Planeta> arbolAux;
			ArbolGeneral<Planeta> jugador = new ArbolGeneral<Planeta> (null);
			c.encolar(arbol);
			while(!c.esVacia()){
				arbolAux = c.desencolar();
				
				if(arbolAux.getDatoRaiz().EsPlanetaDelJugador())
				{	jugador = arbolAux;
					break;
				}
				foreach(ArbolGeneral<Planeta> hijo in arbolAux.getHijos())
					c.encolar(hijo);
			}
			
			return jugador;
		}
	
		// Recorre recursivamente (hacia arriba) los nodos de un ArbolGeneral partiendo desde uno perteneciente a el(buscado),
		// y retorna una lista con aquellos que forman un camino hasta llegar a otro ArbolGeneral (Raíz).
		private List<ArbolGeneral<Planeta>> CaminoDesdeBot (ArbolGeneral<Planeta> buscado, ArbolGeneral<Planeta> arbol,
		                                                 List<ArbolGeneral<Planeta>> camino, ArbolGeneral<Planeta> arbolTotal)
			
		{	foreach(ArbolGeneral<Planeta> hijo in arbol.getHijos())                          //Utilizado en CalcularMovimiento
			{	if( hijo == buscado)
				{	camino.Add(arbol);
					CaminoDesdeBot(arbol, arbolTotal, camino, arbolTotal);
				}

				if(!hijo.esHoja())
					CaminoDesdeBot(buscado, hijo, camino, arbolTotal);
			}
		
			return camino;
		}
		
		// Recorre recursivamente (hacia arriba) los nodos de un ArbolGeneral partiendo desde uno perteneciente a el,
		// y retorna una lista con aquellos que forman un camino desde el primero(raíz) hasta el nodo del que parte(buscado).
		private List<ArbolGeneral<Planeta>> CaminoAlJugador (ArbolGeneral<Planeta> buscado, ArbolGeneral<Planeta> arbol,
		                                                     List<ArbolGeneral<Planeta>> camino, ArbolGeneral<Planeta> arbolTotal)	
		
		{	foreach(ArbolGeneral<Planeta> hijo in arbol.getHijos())                              //Utilizado en CalcularMovimiento
			{	if( hijo == buscado)
				{	camino.Add(arbol);
					CaminoAlJugador(arbol, arbolTotal, camino, arbolTotal);
				}

				if(!hijo.esHoja())
					CaminoAlJugador(buscado, hijo, camino, arbolTotal);
				
			}
			
			List<ArbolGeneral<Planeta>> caminoAJugador = new List<ArbolGeneral<Planeta>>();
			for(int i = camino.Count-1; i>=0; i--) //va sólo hasta el anteúltimo elemento para que la raiz/ancestro no se repita
			{	caminoAJugador.Add(camino[i]);
			}
			caminoAJugador.Add(buscado);
			
			return caminoAJugador;
		}
			
	}
}
