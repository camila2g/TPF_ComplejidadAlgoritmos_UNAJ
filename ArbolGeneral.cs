using System;
using System.Collections.Generic;

namespace DeepSpace
{
	public class ArbolGeneral<T> 
	{
		
		private T dato;
		private List<ArbolGeneral<T>> hijos = new List<ArbolGeneral<T>>();

		public ArbolGeneral(T dato) {
			this.dato = dato;
		}
	
		public T getDatoRaiz() {
			return this.dato;
		}
	
		public List<ArbolGeneral<T>> getHijos() {
			return hijos;
		}
	
		public void agregarHijo(ArbolGeneral<T> hijo) {
			this.getHijos().Add(hijo);
		}
	
		public void eliminarHijo(ArbolGeneral<T> hijo) {
			this.getHijos().Remove(hijo);
		}
	
		public bool esHoja() {
			return this.getHijos().Count == 0;
		}
		
		
		public int altura(){
			if(this.esHoja())
				return 0;
			else{
				int maxAltura = 0;
				foreach(var hijo in this.hijos)
					if(hijo.altura() > maxAltura)
						maxAltura = hijo.altura();
				
				return maxAltura + 1;
			}
		
		}
	
		
		public int nivel(T dato)
		{	Cola<ArbolGeneral<T>> c = new Cola<ArbolGeneral<T>>();
			ArbolGeneral<T> arbolAux;
			
			int nivel = 0;
			
			c.encolar(this);
			c.encolar(null);
			
			while(!c.esVacia()){
				arbolAux = c.desencolar();
				if ((arbolAux != null) && (arbolAux.getDatoRaiz().Equals(dato)))
					break;
				if(arbolAux == null){
					if(!c.esVacia()){
						nivel++;
						c.encolar(null);
					}						
				}
				else{
					foreach (ArbolGeneral<T> ar in arbolAux.getHijos())
						c.encolar(ar);
					}
				}
			return nivel;
		}
		
		
		public bool esHijo (ArbolGeneral<T> hijo , ArbolGeneral<T> padre) //Utilizado en CalcularMovimiento
		{	bool eshijo = false;
			foreach(ArbolGeneral<T> planeta in padre.getHijos())
			{	if(planeta.Equals(hijo))
				{	eshijo = true;
					break;
				}
			}
			
			return eshijo;
		}
	
		
		public  bool esDescendiente (ArbolGeneral<T> arbolRaiz, ArbolGeneral<T> arbolHijo) //Utilizado en CalcularMovimiento
		{	bool esDesc = false;
			Cola<ArbolGeneral<T>> c = new Cola<ArbolGeneral<T>>();
			ArbolGeneral<T> arbolAux;
			
			c.encolar(arbolRaiz);
			while(!c.esVacia()){
				arbolAux = c.desencolar();
				
				if(arbolHijo.Equals(arbolAux))
				{	esDesc = true;
					break;
				}
				foreach(ArbolGeneral<T> hijo in arbolAux.getHijos())
					c.encolar(hijo);
			}
			
			return esDesc;
		}
	
	}
}