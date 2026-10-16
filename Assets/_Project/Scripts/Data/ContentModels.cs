using System;
using System.Collections.Generic;

namespace EnganoColorido.Data
{
    // Contenido editable por la docente en Assets/_Project/Data/*.json.
    // Cada JSON tiene un objeto raíz con una lista (JsonUtility no lee arreglos sueltos).

    [Serializable] public class Ficha
    {
        public string id;
        public string titulo;
        public string pagina;
        public string texto;
    }

    [Serializable] public class Poema
    {
        public string id;
        public string autor;
        public string titulo;
        public string pagina;
        public string rima;
        public string glosario;
        public string[] versos;
    }

    [Serializable] public class Topico
    {
        public string id;
        public string nombre;
        public string idea;
        public string ejemplo;
    }

    [Serializable] public class Opcion
    {
        public string texto;
        public string tag;       // rima, categoría de conector, etc.
    }

    [Serializable] public class Slot
    {
        public string etiqueta;  // lo que se ve junto al hueco
        public string correcta;  // texto exacto de la opción correcta
        public string[] opciones; // opciones propias del hueco (si está vacío se usa step.opciones)
        public int grupo;         // repisa / estrofa, para validar por grupo
    }

    [Serializable] public class Paso
    {
        public string id;
        public string tipo;        // asignar | pregunta | abba
        public string titulo;
        public string enunciado;
        public string plantilla;   // Juicio: texto con {0},{1},{2}
        public string voz;         // Juicio: tesis de El Retrato
        public bool usaInventario; // P3: opciones = objetos recogidos
        public bool estante;       // P5: rótulos según dificultad
        public Opcion[] opciones;
        public Slot[] slots;
        public string correcta;     // tipo pregunta
        public string[] distractores;
        public string[] pistas;     // pistas propias del paso (opcional)
    }

    [Serializable] public class Puzzle
    {
        public string id;
        public string zona;
        public string titulo;
        public string[] fichas;
        public string[] conceptTags;
        public string[] pistas;
        public Paso[] pasos;
    }

    [Serializable] public class Pregunta
    {
        public string id;
        public string enunciado;
        public string correcta;
        public string[] distractores;
        public string concepto;
        public string contexto; // id de poema que se muestra junto a la pregunta
    }

    [Serializable] public class Linea
    {
        public string quien;
        public string texto;
    }

    [Serializable] public class Dialogo
    {
        public string id;
        public Linea[] lineas;
    }

    [Serializable] public class ZonaInfo
    {
        public string id;
        public string nombre;
        public string escena;
        public bool opcional;
    }

    [Serializable] public class Presupuesto
    {
        public string zona;
        public float minutos;
    }

    [Serializable] public class Modo
    {
        public int minutos;
        public float envioTeatroMin;
        public float salidaMin;
        public bool disponible;
        public Presupuesto[] zonas;
    }

    [Serializable] public class SesionConfig
    {
        public string pinDocente = "1234";
        public float toleranciaAtrasoMin = 2;
        public float umbralPistas = 1f;
        public float umbralGuiado = 1.3f;
        public ZonaInfo[] zonas;
        public Modo[] modos;
    }

    // Envoltorios de cada archivo JSON
    [Serializable] public class FichasFile { public List<Ficha> fichas = new(); }
    [Serializable] public class PoemasFile { public List<Poema> poemas = new(); }
    [Serializable] public class TopicosFile { public List<Topico> topicos = new(); }
    [Serializable] public class PuzzlesFile { public List<Puzzle> puzzles = new(); }
    [Serializable] public class DialogosFile { public List<Dialogo> dialogos = new(); }
    [Serializable] public class EvaluacionFile
    {
        public List<Pregunta> diagnostico = new();
        public List<Pregunta> salida = new();
        public Poema poemaSalida;
    }
}
