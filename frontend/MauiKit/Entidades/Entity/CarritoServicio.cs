using Backend.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiKit.Entidades
{
    public static class CarritoServicio
    {
        public static ObservableCollection<Carrito> Items { get; } = new ObservableCollection<Carrito>();
        public static decimal Total { get; private set; }

        public static void AgregarProducto(Producto producto)
        {
            var itemExistente = Items.FirstOrDefault(item => item.Id == producto.idProducto);

            if (itemExistente != null)
            {
                itemExistente.Cantidad++;
            }
            else
            {
                Items.Add(new Carrito
                {
                    Id = producto.idProducto,
                    Nombre = producto.nombre,
                    Descripcion = producto.descripcion,
                    Precio = producto.precio,
                    Cantidad = 1
                });
            }

            CalcularTotal();
        }

        public static void EliminarProducto(Carrito item)
        {
            if (Items.Contains(item))
            {
                if (item.Cantidad > 1)
                {
                    item.Cantidad--;
                }
                else
                {
                    Items.Remove(item);
                }

                CalcularTotal();
            }
        }
        private static void CalcularTotal()
        {
            Total = Items.Sum(item => item.Precio * item.Cantidad);
        }

        public static void Clear()
        {
            Items.Clear();
            Total = 0;
        }
    }
}
