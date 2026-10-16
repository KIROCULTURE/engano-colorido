# ENGAÑO COLORIDO — serious game del Siglo de Oro
### GDD + contenido curricular para desarrollo con Claude Code

**Serious game** en primera persona para **2° medio**: una experiencia educativa de **un bloque de clase** que enseña la unidad del Siglo de Oro del texto escolar de Lengua y Literatura del Mineduc ("Lo divino y lo humano" en el Programa de Estudio). Usa horror psicológico suave y puzzles inspirados en Silent Hill **solo como envoltorio**: cada mecánica existe para enseñar o evaluar un contenido del libro.

> Título tomado del primer verso de Sor Juana Inés de la Cruz ("Este, que ves, engaño colorido…"), poema de la unidad.

---

## 0. Para Claude Code — reglas de este documento

- Este archivo es la **fuente de verdad** de contenido y diseño. `CLAUDE.md` tiene las reglas técnicas.
- **Prioridad: aprendizaje > tiempo de sesión > narrativa > horror.** Si algo compite, gana lo de la izquierda.
- Todo el contenido académico sale del libro (sección 2). Las páginas citadas (p. XX) son del libro, para que la profesora conecte el juego con la clase.
- Los poemas y fragmentos de la sección 2.6 son de **dominio público** (siglos XVI–XVII) y se usan textualmente. Las definiciones están **parafraseadas** del libro (no copiar el libro).
- Silent Hill = **patrones de diseño** (sección 5). Nada de nombres, personajes, criaturas, música, sonidos ni assets de la saga.
- Público menor de edad: sin sangre, gore, armas ni combate; la muerte es tema literario, nunca algo deseable; datos de estudiantes solo locales (sección 8).

---

## 1. Qué es y qué logra

### 1.1 Objetivos de aprendizaje
Al terminar la sesión, el o la estudiante puede:
1. Distinguir **hablante lírico** del autor y reconocer su **actitud** (enunciativa, apostrófica, carmínica).
2. Interpretar el **sentido connotativo** de palabras e imágenes y los **símbolos** barrocos (calavera, reloj, rosa, árbol caído, cárcel).
3. Reconocer la **estructura del soneto** y la función de cada estrofa.
4. Identificar **tópicos literarios** del Siglo de Oro (*carpe diem*, *tempus irreparabile fugit*, *contemptus mundi*, *vanitas vanitatum*, *vita somnium*, *ubi sunt?*).
5. Explicar el **contexto** del Siglo de Oro (Renacimiento → Barroco) y la idea de **desengaño**.
6. Reconocer en *La vida es sueño* el **conflicto dramático**, las **formas del discurso dramático** y los **personajes tipo**.
7. Sostener una postura con **evidencia** y **conectores** (ordenación, consecutivos, adversativos) sobre si hoy necesitamos un "desengaño del mundo".

| OA (Programa 2° medio, U3) | Objetivo del juego |
|---|---|
| OA 4 — análisis de poemas (símbolos, actitud, lenguaje figurado, repeticiones, soneto) | 1, 2, 3 |
| OA 6 — relevancia del Siglo de Oro y su contexto | 4, 5 |
| OA 2 — reflexionar sobre la experiencia humana | narrativa + cierre |
| OA 8 — interpretación con evidencia | 7 |
| OA 9 / OA 14 — argumentación y ensayo | 7 |

### 1.2 Duración: un bloque de clase
- **Modo 90 min (por defecto):** ~57 min de juego dentro de un bloque de 90 (10' de inicio y 20' de cierre los conduce la profesora; ver guía docente, sección 9).
- **Modo 45 min:** ~40 min de juego; se saltan la Capilla y la Torre (sus contenidos no son prerrequisito de nada).
- El modo se elige en el menú del docente. El juego **controla el ritmo** para que todos lleguen al cierre y a la prueba de salida dentro del bloque (sección 4.3).

### 1.3 Ficha técnica
| | |
|---|---|
| Formato | Aventura en primera persona, puzzles, horror psicológico suave |
| Jugadores | 1 por computador (también funciona en parejas) |
| Plataforma | PC Windows de liceo (gráfica integrada), offline |
| Motor | Unity (URP) — ver `CLAUDE.md` |
| Evaluación | Diagnóstico al inicio + prueba de salida + registro por concepto |

---

## 2. Contenido de la unidad (extraído del texto escolar)

### 2.1 Fuente
***Lengua y Literatura 2° medio, Texto del Estudiante*, Santillana** (© 2017; 3ª edición, impresa en octubre de 2019; ISBN 978-956-15-3189-5). PDF oficial descargado desde Currículum Nacional (Mineduc). El tema del Programa "Unidad 3: Lo divino y lo humano (género lírico)" está en **Unidad 2 "El desengaño del mundo", pp. 64–119**.

> **Edición vigente probable:** Santillana © 2020 (4 unidades), donde el tema es la **Unidad 3 "Ideas que cruzan el tiempo", pp. 112–161** (no se pudo leer completa). Comparte el núcleo del juego: Garcilaso, Boscán, Sor Juana, Santa Teresa, Góngora, Calderón y *La vida es sueño*; agrega a María de Zayas, Hipólita de Narváez, Lope de Vega y "La noche boca arriba" (Cortázar). Puede que no incluya *El mágico prodigioso* ni el soneto de Quevedo: ambos se mantienen porque son de dominio público y el de Quevedo funciona igual (o mejor) como texto nuevo en la prueba de salida. **Las páginas que citan las fichas son de la edición 2017; guardarlas en JSON para corregirlas fácilmente si el curso usa la 2020.**

| Sección del libro | Pág. | Contenido |
|---|---|---|
| Entrada de unidad | 64–65 | Pintura *El sueño del caballero* (Antonio de Pereda): vanitas; lema del ángel *Aeterna pungit, cito volat et occidit* ("eternamente hiere, rápido vuela y mata") |
| Para comenzar | 66–73 | *Nicolás Omazur* (Murillo, con calavera); "A una rosa" (Góngora); *La tierra de Jauja* (Lope de Rueda, paso cómico); tópico de la tierra de Jauja |
| Perspectivas literarias | 74–87 | Denotación y connotación; contexto del Siglo de Oro (línea de tiempo + artículo de Ignacio Arellano sobre el s. XVII y el desengaño); poemas: Garcilaso, Santa Teresa, Sor Juana, Boscán; actitud del hablante; símbolos; el soneto |
| Lectura en diálogo | 88–99 | *La vida es sueño*, Jornada II (fragmento); conflicto dramático; formas del discurso dramático; personajes tipo; tópicos literarios |
| Antes de seguir | 100–105 | "Estas que fueron pompa y alegría…" (Calderón); *El mágico prodigioso* (Calderón, la escena del esqueleto); autoevaluación |
| Expresión escrita y oral | 106–111 | Ensayo de 5 párrafos: ¿es necesario hoy un "desengaño del mundo"? (con *The Truman Show* como detonante); conectores; diálogo grupal y escucha activa |
| Para finalizar | 112–119 | "Fue sueño ayer, mañana será tierra…" (Quevedo); *Fuente Ovejuna* (Lope de Vega) |

### 2.2 Contexto: Siglo de Oro y desengaño (pp. 76–78)
- **Siglo de Oro:** periodo de mayor florecimiento de las letras españolas, aprox. **siglos XVI y XVII**. El nombre se discute porque la España de entonces, aunque en el auge de su imperio, vivía un fuerte estancamiento social; lo indiscutible es la calidad e influencia de su literatura.
- **Renacimiento (s. XVI):** transformaciones políticas, sociales y culturales guiadas por la **confianza en la razón**. Garcilaso y Boscán traen el soneto de Italia. **Poesía mística** (Santa Teresa, San Juan de la Cruz): un solo tema, la unión del alma con Dios; estilo sencillo pero cargado de símbolos.
- **Barroco (s. XVII):** visión **pesimista, contradictoria y realista**. España en crisis: despoblación, pobreza, inflación, desprecio del trabajo manual, riqueza concentrada en pocos nobles, guerras, discriminación ("limpieza de sangre").
- **Desengaño:** salir del engaño al conocer la verdad. Motivos frecuentes: la vanidad de la vida, la fugacidad y fragilidad, la delgada línea entre **realidad y apariencia**, la muerte y el tiempo destructor. Convive con su opuesto: el gusto por el lujo y la ostentación.
- **Estética barroca:** exageración, ruptura del equilibrio, búsqueda de impresionar, artificio, antítesis violenta, metáfora extraña.
- **Vanitas:** género de pintura barroca que muestra lo breve y frágil de la vida con objetos simbólicos (Pereda, Murillo).
- **Hitos útiles para el juego:** *La vida es sueño* (1636) es la obra más célebre de Calderón; Sor Juana es la máxima figura del Barroco en Hispanoamérica; el Siglo de Oro se cierra simbólicamente en 1700 con la muerte de Carlos II.

### 2.3 Lírica (pp. 74, 84–87)
- **Denotación / connotación:** significado literal y objetivo / significado nuevo que la palabra adquiere en un contexto. La lírica privilegia la connotación. Ej. del libro: en "A una rosa", la rosa connota la existencia humana (bella pero breve) y la "robusta mano" y el "tirano" connotan la muerte.
- **Hablante lírico:** voz que expresa ideas y emociones en el poema; **no es el autor**.
- **Actitudes del hablante:** **enunciativa** (cuenta hechos y muestra su sentir ante ellos), **apostrófica** (se dirige a alguien o algo para expresarle su sentir), **carmínica** o de la canción (abre su mundo interior). Ejemplos del libro: Boscán, "Durmiendo, en fin, fui bienaventurado…" → carmínica; Garcilaso, "coged de vuestra alegre primavera…" → interpela a un "tú" (apostrófica).
- **Símbolos:** elementos que representan ideas complejas; algunos son culturales y otros los crea el autor. Tabla del libro:

| Símbolo | Significado |
|---|---|
| Calavera | La muerte y lo transitorio de la vida |
| Reloj | La fugacidad del tiempo y, por lo tanto, de la vida |
| Rosa | La belleza y la juventud |
| Árbol caído | El paso inexorable del tiempo, que todo lo derriba |
| Cárcel (Santa Teresa) | El cuerpo como prisión del alma; la vida como opresión |

- **Soneto (p. 87):** nace en Italia (s. XIV, Petrarca) y lo cultivan los poetas del Siglo de Oro. **14 versos endecasílabos** (11 sílabas), **dos cuartetos** con la misma rima (**ABBA ABBA**) y **dos tercetos** de rima variable. Función de cada estrofa (ejemplo del libro con el Soneto XXIII):
  1. 1° cuarteto: presenta el tema u objeto (la juventud).
  2. 2° cuarteto: lo amplía o profundiza (el aspecto juvenil).
  3. 1° terceto: reflexión, expresa un sentimiento (hay que aprovechar la juventud).
  4. 2° terceto: conclusión o cierre (la muerte es inevitable).
- **Interpretaciones propuestas por el libro (p. 84):** Soneto XXIII = fragilidad de la juventud ante el tiempo; "Vivo sin vivir en mí" = la vida terrenal como prisión y anhelo de encuentro con Dios; "Este, que ves…" = crítica al arte engañoso que oculta la muerte; "Dulce soñar…" = la dicha solo existe en el sueño, aunque sea engaño.

### 2.4 Teatro (pp. 88, 96–98)
- **Argumento de *La vida es sueño*:** un oráculo anuncia que el príncipe Segismundo será un tirano y derrotará a su padre, el rey Basilio de Polonia. Basilio lo declara muerto y lo encierra en una torre al cuidado de Clotaldo. Años después lo prueba: lo duerme, lo lleva al palacio, Segismundo actúa con soberbia y vuelve a la torre creyendo que todo fue un sueño. El pueblo lo libera; Segismundo vence a su padre, lo perdona y demuestra que la voluntad puede vencer al destino. Temas: **libre albedrío**, apariencia vs. realidad, ejercicio del poder.
- **Conflicto dramático:** un personaje persigue un fin y choca con un obstáculo; tipos: debate moral ante el destino, dos concepciones de mundo, lucha interna, individuo vs. sociedad, rivalidad entre personajes. En *La vida es sueño*: **libre albedrío (Segismundo) vs. destino (Basilio)**, más la **lucha interna** de Segismundo.
- **Formas del discurso dramático:** **diálogo** (forma principal), **acotación** (orienta la puesta en escena, entre paréntesis), **aparte** (el personaje habla al público sin que lo oigan los demás), **monólogo** (expresa su interior sin esperar respuesta); el **soliloquio** es una forma emparentada.
- **Personajes tipo:** **galán** (Segismundo), **dama** (Rosaura), **gracioso** (Clarín), **rey** (Basilio), **caballero**, **poderoso** (el Comendador en *Fuente Ovejuna*), **villano** (Frondoso).

### 2.5 Tópicos literarios (p. 98)
Un tópico (*topos*, "lugar común") es un tema que se repite desde la Antigüedad; tiene nombre en latín y su reconocimiento ayuda a interpretar.

| Tópico | Idea | Ejemplo del libro |
|---|---|---|
| *Tempus irreparabile fugit* | El tiempo se escapa sin remedio; la muerte es irremediable | Jorge Manrique, *Coplas* |
| *Carpe diem* | Aprovecha el momento, sobre todo la juventud | Garcilaso, Soneto XXIII |
| *Contemptus mundi* | Desprecio de lo que ofrece el mundo | Santa Teresa, "Vivo sin vivir en mí" |
| *Vanitas vanitatum* | Las cosas son engañosas; hay que dejarlas por otras más elevadas | Sor Juana, "Este, que ves…" |
| *Vita somnium* | La vida como experiencia irreal, ficción pasajera | Calderón, *La vida es sueño* |
| *Ubi sunt?* | Pregunta por personas o realidades desaparecidas | Garcilaso, Égloga I |
| Tierra de Jauja (p. 69) | Lugar ideal de abundancia donde todo es posible sin esfuerzo | Lope de Rueda, *La tierra de Jauja* |

### 2.6 Corpus de dominio público (usar textualmente)

**Luis de Góngora — "A una rosa"** (p. 67). Glosario: *lucido* = llamativo por su belleza; *lozano* = vigoroso, juvenil; *vano* = ilusorio; *desvanecido* = deshecho; *dilatar* = aplazar.
```
Ayer naciste y morirás mañana.
Para tan breve ser, ¿quién te dio vida?
¿Para vivir tan poco estás lucida,
y para no ser nada estás lozana?

Si te engañó tu hermosura vana,
bien presto la verás desvanecida,
porque en tu hermosura está escondida
la ocasión de morir muerte temprana.

Cuando te corte la robusta mano,
ley de la agricultura permitida,
grosero aliento acabará tu suerte.

No salgas, que te aguarda algún tirano;
dilata tu nacer para tu vida,
que anticipas tu ser para tu muerte.
```

**Garcilaso de la Vega — Soneto XXIII** (p. 79). Rima ABBA ABBA CDE DCE. Glosario: *vena* = parte de una materia que se distingue por calidad o color; *enhiesto* = erguido.
```
En tanto que de rosa y azucena
se muestra la color en vuestro gesto,
y que vuestro mirar ardiente, honesto,
con clara luz la tempestad serena;

y en tanto que el cabello, que en la vena
del oro se escogió, con vuelo presto,
por el hermoso cuello blanco, enhiesto,
el viento mueve, esparce y desordena;

coged de vuestra alegre primavera
el dulce fruto, antes que el tiempo airado
cubra de nieve la hermosa cumbre.

Marchitará la rosa el viento helado,
todo lo mudará la edad ligera,
por no hacer mudanza en su costumbre.
```

**Santa Teresa de Jesús — "Vivo sin vivir en mí…" (fragmento)** (p. 80). Glosario: *aquesta* = esta; *cautivo* = prisionero; *do* = donde.
```
Vivo sin vivir en mí,
y tan alta vida espero,
que muero porque no muero.

Aquesta divina unión,
del amor con que yo vivo,
hace a Dios ser mi cautivo,
y libre mi corazón;
mas causa en mí tal pasión,
ver a Dios mi prisionero,
que muero porque no muero.

¡Ay, qué larga es esta vida!
¡Qué duros estos destierros,
esta cárcel, estos hierros
en que el alma está metida!
Solo esperar la salida
me causa dolor tan fiero,
que muero porque no muero.
```
> Cuidado temático (obligatorio en la ficha): el "morir" de la mística es el **anhelo de unión con Dios** en el contexto religioso del s. XVI. El juego nunca presenta la muerte como salida ni como algo deseable.

**Sor Juana Inés de la Cruz — "Este, que ves, engaño colorido…"** (p. 82). Rima ABBA ABBA CDC DCD. Glosario: *primor* = destreza; *silogismo* = razonamiento de dos premisas a una conclusión; *vano artificio* = engaño sin sentido; *caduco* = destinado a caer.
```
Este, que ves, engaño colorido,
que del arte ostentando los primores,
con falsos silogismos de colores
es cauteloso engaño del sentido;

este, en quien la lisonja ha pretendido
excusar de los años los horrores
y venciendo del tiempo los rigores
triunfar de la vejez y del olvido,

es un vano artificio del cuidado,
es una flor al viento delicada,
es un resguardo inútil para el hado:

es una necia diligencia errada,
es un afán caduco y, bien mirado,
es cadáver, es polvo, es sombra, es nada.
```

**Juan Boscán — "Dulce soñar y dulce congojarme…"** (p. 83). Rima ABBA ABBA CDC DCD. "Dulce" se repite 5 veces (inicio de los versos 1, 3, 5 y 7, y dos veces en el verso 1). Glosario: *congojarme* = angustiarme; *figurarme* = imaginarme; *importunar* = molestar; *asentar* = instalar; *bienaventurado* = feliz.
```
Dulce soñar y dulce congojarme,
cuando estaba soñando que soñaba;
dulce gozar con lo que me engañaba,
si un poco más durara el engañarme;

dulce no estar en mí, que figurarme
podía cuanto bien yo deseaba,
dulce placer, aunque me importunaba
que alguna vez llegaba a despertarme.

¡Oh sueño, cuánto más leve y sabroso
me fueras si vinieras tan pesado
que asentaras en mí con más reposo!

Durmiendo, en fin, fui bienaventurado,
y es justo en la mentira ser dichoso
quien siempre en la verdad fue desdichado.
```

**Pedro Calderón de la Barca — *La vida es sueño*, Jornada II (fragmentos, pp. 89–94)**
```
(Despierta.)
SEGISMUNDO.— Mas ¡ay de mí!, ¿dónde estoy?

CLOTALDO (Aparte.).— A mí me toca llegar
a hacer la deshecha ahora.
¿Es ya de despertar hora?
SEGISMUNDO.— Sí, hora es ya de despertar.

CLOTALDO.— […] Segismundo; que aun en sueños
no se pierde el hacer bien.
(Vase.)
SEGISMUNDO.— Es verdad; pues reprimamos
esta fiera condición,
esta furia, esta ambición
por si alguna vez soñamos.

SEGISMUNDO.— […]
¿Qué es la vida? Un frenesí.
¿Qué es la vida? Una ilusión,
una sombra, una ficción,
y el mayor bien es pequeño;
que toda la vida es sueño,
y los sueños, sueños son.
```
> Atribución verificada en el libro (p. 93): **"que aun en sueños / no se pierde el hacer bien" lo dice Clotaldo**, y Segismundo le responde "Es verdad…".

**Calderón — "Estas que fueron pompa y alegría…"** (p. 100). Rima ABBA ABBA CDC DCD. Glosario: *pompa* = esplendor; *albor* = primera luz del día; *grana* = rojo; *escarmiento* = castigo, lección; *botón* = brote de la flor.
```
Estas que fueron pompa y alegría
despertando al albor de la mañana,
a la tarde serán lástima vana
durmiendo en brazos de la noche fría.

Este matiz que al cielo desafía,
Iris listado de oro, nieve y grana,
será escarmiento de la vida humana:
¡tanto se emprende en término de un día!

A florecer las rosas madrugaron,
y para envejecerse florecieron:
cuna y sepulcro en un botón hallaron.

Tales los hombres sus fortunas vieron:
en un día nacieron y espiraron;
que pasados los siglos, horas fueron.
```

**Calderón — *El mágico prodigioso*, Jornada III (escena clave, p. 101).** Cipriano, que vendió su alma para conquistar a Justina, la descubre y encuentra un esqueleto que le dice:
```
ESQUELETO.— Así, Cipriano, son
todas las glorias del mundo.
```

**Francisco de Quevedo — "Fue sueño ayer, mañana será tierra…"** (p. 112) — **reservado para la prueba de salida** (texto nuevo para medir transferencia; no aparece antes en el juego). Rima ABBA ABBA CDC DCD. Glosario: *presumir* = suponer; *sumo* = máximo; *despeñado* = arrojado al abismo; *azada* = herramienta para cavar; *jornal* = paga.
```
¡Fue sueño ayer, mañana será tierra!:
poco antes nada, y poco después humo,
¡y destino ambiciones! ¡y presumo,
apenas junto al cerco que me cierra!

Breve combate de importuna guerra,
en mi defensa, soy peligro sumo;
y mientras con mis armas me consumo,
menos me hospeda el cuerpo, que me entierra.

Ya no es ayer; mañana no ha llegado;
hoy pasa, y es, y fue, con movimiento
que a la muerte me lleva despeñado.

Azadas son la hora y el momento
que, a jornal de mi pena y mi cuidado,
cavan en mi vivir mi monumento.
```

### 2.7 Expresión escrita y oral (pp. 106–111)
- **Ensayo:** expresa las ideas del autor sobre un tema; es crítico y argumentativo, pero busca **hacer reflexionar** más que persuadir; deja el tema abierto; usa citas y ejemplos. Alfonso Reyes lo llamó "el centauro de los géneros".
- **Conectores:**
  - **De ordenación** (en primer lugar, para empezar, a continuación, finalmente): van al inicio de la oración y seguidos de coma.
  - **Consecutivos** (por lo tanto, por ende, en consecuencia, así pues): introducen una consecuencia; suelen ir precedidos de coma.
  - **Adversativos** (sin embargo, no obstante, en cambio, por el contrario): oponen ideas; van entre comas o con coma después si inician la oración.
- **Diálogo y escucha activa:** respetar turnos, retomar lo dicho por otros, atender a lo verbal, paraverbal y no verbal.

### 2.8 Textos del libro que NO se reproducen
*La tierra de Jauja* (la versión del libro es una adaptación moderna), *Fuente Ovejuna* (escena con violencia; fuera del foco lírico), los comentarios de cuadros y el artículo de Arellano (solo paráfrasis), *The Truman Show* (película con derechos: solo como referencia en la guía docente). Las pinturas de Pereda y Murillo son del s. XVII: se pueden **recrear** como escenas 3D originales inspiradas en su composición.

---

## 3. Narrativa

### 3.1 Premisa
La noche antes de la prueba de Siglo de Oro, Ale se queda en el teatro del liceo ensayando el monólogo de Segismundo. Repasa con una app de preguntas (**diagnóstico**), abre un filtro "Dorado" frente al espejo del camarín y se duerme. Despierta a las **23:59**: la camanchaca entró al liceo, los relojes están detenidos y su reflejo ya no le imita. Su versión filtrada, **El Retrato**, le ofrece quedarse en un mundo perfecto, dorado y sin esfuerzo (una **tierra de Jauja** de likes). Para despertar, Ale tiene que **desengañarse**: leer el liceo como un poema barroco.

**Pregunta central (la misma del ensayo del libro, p. 106):** *¿necesitamos hoy un "desengaño del mundo"?*

### 3.2 Personajes
- **Ale** (jugable): 2° medio, **género neutro** (nunca adjetivos con género para Ale; se le ve solo en reflejos). Ironía adolescente, chilenismos suaves en su voz interna. Su conflicto: vive pendiente de la imagen que muestra.
- **El Retrato** (antagonista): su versión con filtro. Seduce, no ataca. Su tesis es la de Boscán: *"es justo en la mentira ser dichoso"*. Se comunica por **stories y notificaciones** que reescriben tópicos en lenguaje de redes ("Vive hoy, mañana hay nieve 🌹❄️ #CarpeDiem").
- **Don Ruperto** (guía): auxiliar del liceo, cálido y con humor de campo; el único otro humano en la niebla. Funciona como el **gracioso** sabio y como sistema de ayuda cuando Ale se atrasa.
- **La profe Inés** (homenaje a Sor Juana): nunca aparece; deja **fichas** (el sistema de enseñanza, sección 4.2).
- **Las Horas:** siluetas con un reloj sin manecillas por cara. No hieren: **roban tiempo** y aumentan la tensión. En modo Calma no existen.

### 3.3 Dos mundos = lo humano y lo divino
| | Mundo de Niebla (lo humano) | Mundo Dorado (la apariencia) |
|---|---|---|
| Disparador | Estado normal | Suena la **campana** del liceo |
| Aspecto | El liceo de siempre, vacío, camanchaca, relojes en 23:59 | Pan de oro como un retablo, velas, flores marchitas, objetos vanitas, inscripciones en latín de los tópicos |
| Sentido | Lo real, lo pasajero | El brillo engañoso: vanitas + tierra de Jauja |
> Nada de óxido, sangre ni carne: el "otro mundo" es **barroco**, no industrial.

### 3.4 Finales
Tras el Juicio, Ale elige:
- **Despertar (desengaño):** 07:59, Don Ruperto abre el teatro; Ale dice el monólogo. Última decisión sin moraleja forzada: publicar o no la foto sin filtro.
- **Dulce soñar:** Ale se queda; plano final de un retrato nuevo en la galería, sonriendo para siempre, con el verso de Boscán. Melancólico, **sin muerte**.

Ambos finales llevan a **la prueba** (prueba de salida). La elección se registra (sirve para el diálogo del cierre).

---

## 4. Diseño instruccional

### 4.1 Estructura de la sesión
| # | Momento | Modo 90 | Modo 45 | Contenido (p. del libro) |
|---|---|---|---|---|
| D | **Diagnóstico** (app de repaso de Ale) | 3' | 2' | 5 preguntas (4.4) |
| 0 | Prólogo: camarín | 3' | 2' | Hablante lírico; desengaño (pp. 76–78, 86) |
| 1 | Patio de las rosas | 8' | 7' | Denotación/connotación, símbolo rosa, *carpe diem* (pp. 67, 74, 79, 86) |
| H | Las tres radios (hub) | 3' | 3' | Actitudes del hablante (p. 86) |
| 2 | Capilla | 7' | — | Santa Teresa, repetición/estribillo, cárcel, *contemptus mundi*, mística (pp. 80–81, 86) |
| 3 | Galería | 9' | 8' | Vanitas (Pereda, Murillo), símbolos, Sor Juana, *El mágico prodigioso* (pp. 64–66, 82, 101) |
| 4 | Biblioteca | 8' | 7' | Soneto (Boscán), *vita somnium* (pp. 83, 87) |
| 5 | Torre del reloj | 6' | — | Calderón "Estas que fueron…", *tempus fugit*, tópicos (pp. 98, 100) |
| F | Teatro: libreto + Juicio + Tu verso | 9' | 8' | Teatro (pp. 88–98) y argumentación (pp. 106–111) |
| S | **Prueba de salida** | 4' | 3' | 8 preguntas (4.4) |
| | **Total de juego** | **≈ 60'** | **≈ 40'** | |

### 4.2 Fichas de la profe Inés (enseñar → practicar → evaluar)
Antes de cada puzzle hay una ficha (máx. 80 palabras, lenguaje de 2° medio, con página del libro). Se guardan en el **Cuaderno** del celular para consultarlas en cualquier momento. Fichas mínimas: Hablante lírico · Desengaño y Barroco · Denotación/connotación · Símbolos barrocos · Actitudes del hablante · Poesía mística · Vanitas · Soneto · Tópicos · *La vida es sueño* (argumento) · Formas del discurso dramático · Personajes tipo · Conectores.

### 4.3 Control de ritmo (para que calce en el bloque)
- Cada zona tiene un **presupuesto de tiempo** (tabla 4.1). Al pasar el 100 % las pistas se adelantan; al 130 %, Don Ruperto ofrece mostrar el camino y el puzzle queda en modo guiado (se registra como "resuelto con ayuda").
- Si la sesión va atrasada, el juego **salta automáticamente** las zonas opcionales del modo (Capilla, luego Torre) y lo informa con una nota en el Cuaderno.
- A los **minutos 47 (modo 90) / 29 (modo 45)**, pase lo que pase, el juego lleva a Ale al Teatro para el Juicio, y luego a la prueba de salida.
- Pistas en **3 niveles** en todo puzzle. Dificultad de acertijos *Fácil/Normal/Difícil* (Fácil por defecto).

### 4.4 Evaluación
**Diagnóstico (antes) y prueba de salida (después).** Las preguntas D1–D5 y S1–S5 miden lo mismo con ejemplos distintos (ganancia de aprendizaje). S6–S8 miden **transferencia** con un poema nuevo (Quevedo, p. 112). Orden de alternativas aleatorio.

| # | Pregunta | Correcta | Distractores | Concepto |
|---|---|---|---|---|
| D1 | ¿Quién habla en un poema? | El hablante lírico, una voz ficticia | Siempre el autor · El narrador · El lector | hablante |
| D2 | En "la rosa es la vida: bella y breve", *rosa* se usa en sentido… | Connotativo | Denotativo · Literal · Científico | connotación |
| D3 | Un soneto tiene… | 14 versos: dos cuartetos y dos tercetos | 10 versos · Cuatro estrofas de cuatro versos · Versos libres sin rima | soneto |
| D4 | *Carpe diem* significa… | Aprovecha el momento | Desprecio del mundo · La vida es sueño · ¿Dónde están? | tópicos |
| D5 | En el arte del Siglo de Oro, una calavera junto a un reloj simboliza… | La muerte y lo pasajero de la vida | La riqueza · La ciencia · El amor | símbolos |
| S1 | En "Vivo sin vivir en mí", quien dice "que muero porque no muero" es… | El hablante lírico | Santa Teresa contando un hecho real · Un personaje de teatro · El narrador | hablante |
| S2 | En "A una rosa", "la robusta mano" connota… | La muerte | Un jardinero · La fuerza física · El trabajo | connotación |
| S3 | Los cuartetos de un soneto clásico riman… | ABBA ABBA | ABAB CDCD · AABB · No riman | soneto |
| S4 | "¿Qué es la vida? Una ilusión, / una sombra, una ficción" expresa el tópico… | *Vita somnium* | *Carpe diem* · *Ubi sunt?* · *Contemptus mundi* | tópicos |
| S5 | En un cuadro vanitas, el reloj representa… | La fugacidad del tiempo y de la vida | La paciencia · La riqueza · El orden | símbolos |
| S6 | (Quevedo, cuartetos) Los dos primeros versos se asocian al tópico… | *Tempus irreparabile fugit* | *Carpe diem* · Tierra de Jauja · *Ubi sunt?* | transferencia |
| S7 | (Quevedo, poema completo) La actitud del hablante es… | Carmínica: expresa su mundo interior | Enunciativa · Apostrófica | transferencia (el libro deja la pregunta abierta en p. 112: confirmar con la docente) |
| S8 | (Quevedo) ¿Qué visión del Barroco comparte el poema? | La vida es breve y el tiempo lleva a la muerte (desengaño) | La vida es una fiesta sin fin · El amor lo vence todo · La naturaleza es armónica y eterna | transferencia |

**Durante el juego** se registra por puzzle: intentos, pistas, tiempo, resuelto (solo / con ayuda), y `conceptTags`.

---

## 5. Referentes de Silent Hill (solo patrones de diseño)

| Patrón de la saga | Adaptación educativa |
|---|---|
| Niebla densa | Camanchaca dentro del liceo (también abarata el render) |
| Mundo alternativo tras una sirena | Mundo Dorado barroco al sonar la **campana** |
| Radio con estática cerca de enemigos | El **celular** de Ale hace estática procedural cerca de Las Horas |
| Linterna como luz principal | Linterna del celular |
| Acertijos basados en poemas crípticos (piano, libros, relojes) | Órgano del estribillo (P2), estante del soneto (P5), reloj de las tres horas (P6) |
| Dificultad de acertijos separada de la de acción | *Acertijos* (Fácil/Normal/Difícil) y *Presencias* (Calma/Suave) |
| Memos con pistas y lore | Fichas de la profe Inés + stories de El Retrato |
| Mapa que el personaje anota | Mapa que se marca solo: X cerrado, ? pendiente, ✓ resuelto |
| Puntos de guardado distintivos | Velas rojas |
| Maniquíes inquietantes | Maniquíes del teatro con máscaras de personajes tipo (P7) |
| Mundo que refleja el conflicto interno | El liceo dorado es la vanidad de Ale hecha arquitectura |
| Finales múltiples | Despertar / Dulce soñar |

**Prohibido:** nombres, personajes, criaturas, frases, música, efectos, logos o assets de Silent Hill/Konami.

---

## 6. Puzzles (solución, distractores, pistas)

Reglas generales: ficha previa siempre; 3 niveles de pista; nunca bloquear más que el presupuesto de la zona (4.3); cada intento se registra.

### P1 — El candado de las rosas (Patio) · connotación, símbolo, *carpe diem*
- **Ficha previa:** denotación/connotación con "A una rosa" (p. 74).
- **A. Candado de 4 diales:** placa con "A una rosa" y el 1° terceto de Garcilaso. Cada dial muestra una imagen del poema y hay que girarlo hasta su sentido connotativo:
  - **Rosa → La existencia humana: bella y breve**
  - **Robusta mano / tirano → La muerte**
  - **Primavera → La juventud**
  - **Nieve en la cumbre → La vejez (canas)**
  - Distractores en cada dial: Un jardinero · El invierno · El amor · La riqueza.
- **B. El momento justo (*carpe diem* jugable):** las rosas del invernadero son botones en la Niebla y están marchitas en el Dorado; **solo florecen ~4 s durante el cambio de mundo**. Ale debe cortar una en ese instante. Sin castigo: la campana vuelve a sonar cada 20 s. La rosa se usa en P3.
- **Pistas:** (1) "¿Qué estación del año es la juventud?" (2) "Las canas son blancas como la nieve." (3) Marca el verso de cada imagen.

### H — Las tres radios (hub) · actitudes del hablante
- **Ficha previa:** actitudes (p. 86).
- Tres radios; al sintonizar cada una (dial con estática) se escucha un fragmento (TTS/voz) y se le pega una etiqueta:
  - Boscán, "Durmiendo, en fin, fui bienaventurado, / y es justo en la mentira ser dichoso / quien siempre en la verdad fue desdichado" → **Carmínica** (ejemplo del libro)
  - Garcilaso, "coged de vuestra alegre primavera / el dulce fruto" → **Apostrófica** (ejemplo del libro: interpela a un "tú")
  - Calderón, "Estas que fueron pompa y alegría / despertando al albor de la mañana, / a la tarde serán lástima vana" → **Enunciativa** (describe las rosas). *Clasificación propuesta, no viene del libro: confirmar con la docente.*
- Abre la puerta de la Galería.

### P2 — El órgano del estribillo (Capilla, solo modo 90) · repetición, símbolo de la cárcel, mística
- **Ficha previa:** poesía mística (p. 80) + cuidado temático.
- Tres placas con las estrofas de Santa Teresa. Nota en el atril: *"El alma repite lo que la desvela. Tócalo al final de cada estrofa."* Las teclas del órgano tienen palabras; hay que tocar en orden **"que · muero · porque · no · muero"** (el verso que se repite).
- Luego, la reja de la cripta pregunta: **"¿Qué representa la cárcel en el poema?"** → **El cuerpo como prisión del alma** (distractores: Una cárcel real donde estuvo la autora · El castigo de Dios · La pobreza). Y: **"¿Qué tópico expresa?"** → ***Contemptus mundi*** (distractores: *carpe diem*, *ubi sunt?*).
- **Pistas:** (1) "Busca el verso que aparece en todas las placas." (2) Resalta los finales de estrofa. (3) Muestra la primera palabra.

### P3 — El bodegón del caballero (Galería) · símbolos vanitas
- **Ficha previa:** vanitas con *El sueño del caballero* y *Nicolás Omazur* (pp. 64–66).
- Recreación 3D de la mesa de *El sueño del caballero* con huecos. Sobre ella, la cinta del ángel: *Aeterna pungit, cito volat et occidit*. Nota: *"Pon lo que dice que todo termina, que el tiempo huye, que la belleza se marchita y que el poder no dura."*
  - Solución: **calavera** (laboratorio), **reloj** (sala de profesores), **rosa** (de P1), **corona** (utilería del teatro).
  - Distractores repartidos por el liceo: paloma, ancla, llave, pelota.
- **Pistas:** (1) Abre la ficha de símbolos. (2) Recuerda la rosa del patio. (3) Marca en el mapa dónde está cada objeto.

### P4 — El retrato que miente (Galería) · Sor Juana, anáfora y gradación, *vanitas*
- **Ficha previa:** "Este, que ves, engaño colorido…" (p. 82).
- **A.** Cuatro marcos con el retrato de Ale cada vez más deteriorado: colgarlos según el último verso: **cadáver → polvo → sombra → nada** (el último marco queda vacío).
- **B.** Pregunta: "¿Qué hace el retrato según los cuartetos?" → **Aparenta vencer al tiempo y al olvido** (distractores: Muestra la verdad del rostro · Celebra la vejez).
- **C. Escena de *El mágico prodigioso*:** una figura velada con la ropa de Ale espera al final de la galería. Al quitarle el velo aparece un esqueleto (sin sangre) que dice: *"Así, Cipriano, son todas las glorias del mundo"*. Ficha de *El mágico prodigioso* (p. 101). En Calma: sin sonido brusco, transición lenta.
- **Pistas:** (1) "Cada palabra es menos que la anterior." (2) Muestra el verso. (3) Cuelga el primero.

### P5 — El estante del soneto (Biblioteca) · estructura del soneto, *vita somnium*
- **Ficha previa:** el soneto (p. 87).
- 14 libros con un verso de Boscán en el lomo, desordenados; estante de 4 repisas (4/4/3/3). Validación por repisa.
  - Solución: el soneto en su orden (2.6).
  - *Fácil:* lomos coloreados por rima y repisas rotuladas ABBA / ABBA / CDC / DCD. *Normal:* solo rótulos. *Difícil:* sin rótulos; pista: "dulce" abre los versos 1, 3, 5 y 7.
- Después, rotular la función de cada estrofa: **tema → amplía → reflexión → conclusión**.
- Pregunta: "¿Por qué se repite 'dulce'?" → **Para enfatizar el placer del sueño** (distractores: Por error del poeta · Solo para rimar).
- Pregunta de tópico: → ***Vita somnium***.

### P6 — El reloj de las tres horas (Torre, solo modo 90) · *tempus irreparabile fugit*
- **Ficha previa:** tópicos (p. 98).
- El reloj tiene tres manecillas: **Albor**, **Tarde**, **Noche**. Hay tres rosas: abierta, marchita, caída. Placa: 1° cuarteto de "Estas que fueron pompa y alegría…". Solución:
  - **Albor → rosa abierta** ("pompa y alegría despertando al albor de la mañana")
  - **Tarde → rosa marchita** ("a la tarde serán lástima vana")
  - **Noche → rosa caída** ("durmiendo en brazos de la noche fría")
- Péndulo: "¿Qué dice el último terceto de los seres humanos?" → **Que viven y mueren tan rápido como las rosas** (distractores: Que son eternos · Que dominan la naturaleza).
- Mini-emparejamiento de tópicos con sus versos (tabla 2.5): las inscripciones latinas del Mundo Dorado se iluminan al acertar.
- Al resolver, el reloj marca 00:00 y Las Horas se detienen.

### P7 — El libreto de Segismundo (Teatro) · teatro del Siglo de Oro
- **Fichas previas:** argumento de *La vida es sueño*, formas del discurso, personajes tipo (pp. 88, 97–98).
- **A.** Cuatro hojas del libreto sin rótulo; pegar la etiqueta correcta:
  - "(Despierta.)" → **Acotación**
  - "CLOTALDO (Aparte.).— A mí me toca llegar / a hacer la deshecha ahora." → **Aparte**
  - "CLOTALDO.— ¿Es ya de despertar hora? / SEGISMUNDO.— Sí, hora es ya de despertar." → **Diálogo**
  - "¿Qué es la vida? Un frenesí…" → **Monólogo**
- **B.** Cuatro maniquíes con ropa de los personajes; ponerles su máscara: **Segismundo → Galán**, **Rosaura → Dama**, **Clarín → Gracioso**, **Basilio → Rey**.
- **C.** "¿Cuál es el conflicto central?" → **Libre albedrío (Segismundo) vs. destino (Basilio)**.

### FINAL — El Juicio del Espejo (Teatro) · argumentación con evidencia y conectores
- **Ficha previa:** conectores (p. 108).
- 3 rondas. El Retrato dice una tesis; Ale arma su respuesta llenando **3 huecos**: una concesión ya escrita + **conector** + **evidencia** (verso del Cuaderno) + **conector** + conclusión ya escrita. Error = pierde Tiempo y El Retrato contraataca; 2 errores = pista.

| Ronda | Tesis de El Retrato | Respuesta correcta de Ale |
|---|---|---|
| 1 | "La juventud es lo único que vale: congélala en una foto." | "Es cierto que la juventud es bella; **sin embargo**, Garcilaso advierte que *'todo lo mudará la edad ligera'*; **por lo tanto**, hay que vivirla, no congelarla." |
| 2 | "Un retrato perfecto vence al tiempo y al olvido." | "**En primer lugar**, Sor Juana dice que el retrato es *'un resguardo inútil para el hado'*; **en consecuencia**, la imagen solo aparenta vencer al tiempo." |
| 3 | "Es justo en la mentira ser dichoso." (Boscán) | "Puede que la vida sea sueño; **no obstante**, como dice Clotaldo, *'aun en sueños / no se pierde el hacer bien'*; **por ende**, lo que hacemos importa aunque todo sea apariencia." |

- Distractores: conectores de la categoría equivocada (p. ej., "por lo tanto" donde va una oposición) y versos reales pero irrelevantes. Feedback breve que nombra la categoría ("Ese conector introduce una consecuencia, no una oposición").

### CIERRE — Tu verso (libro p. 85, actividad 7)
- Ale escribe **una estrofa de cuatro versos sobre cómo percibe la vida**. Dos caminos:
  - **Armar** un cuarteto con rima **abba** desde un banco (todos de 8 sílabas, verificados): rima *-ado*: "No quiero un rostro pintado" · "el reloj sigue a mi lado" · "y el ayer ya se ha marchado"; rima *-ida*: "prefiero vivir mi vida" · "con la luz que no se olvida" · "aunque el filtro me lo pida". Distractores sin rima: "quiero quedarme en el sueño" · "todo lo cambia el tiempo". Se valida solo el esquema abba.
  - **Escribir** cuatro versos propios (sin validación; se guardan para la profesora).
- La estrofa aparece en los créditos.

---

## 7. Sistemas
- **Controlador en primera persona:** caminar, agacharse, linterna, interactuar (raycast), inspeccionar objetos.
- **Celular (UI diegética):** Cuaderno (fichas y versos), Mapa, Inventario, Stories, Pensar (pistas), Ajustes.
- **Tiempo (vida):** reloj de arena; lo consumen Las Horas y los errores del Juicio. En cero, Ale despierta en la última vela (sin muerte ni game over). Nunca bloquea el avance.
- **Cambio de mundo:** variantes Niebla/Dorado por sala; transición suave (sin destellos).
- **Las Horas:** patrulla simple con NavMesh; persiguen al oír correr; se pierden si Ale se agacha o se esconde. No hay persecuciones largas ni zonas bloqueadas por ellas.
- **Guardado:** velas rojas + autoguardado al terminar cada zona (si se corta la clase, se retoma).
- **Ritmo:** sección 4.3.
- **Accesibilidad:** subtítulos siempre activos, tamaño de texto, sensibilidad, FOV, sin destellos, modo Calma, opción sin sustos repentinos, lectura en voz alta de poemas (TTS).

## 8. Datos y privacidad (estudiantes menores de edad)
- Al iniciar: **alias** (no nombre completo) + curso + modo. Sin cuentas, sin internet, sin analítica externa.
- Por estudiante se guarda en local: alias, curso, modo, fecha; puntaje D1–D5, S1–S8 y ganancia (S1–S5 menos D1–D5); por puzzle: intentos, pistas, tiempo, solo/con ayuda; final elegido; estrofa de Tu verso.
- Exportar **CSV + JSON** a una carpeta configurable (p. ej., carpeta compartida o pendrive). Menú docente protegido con un PIN simple.
- Pantalla final para la profesora: resumen del curso por concepto (si se juntan los CSV en la misma carpeta).

## 9. Guía docente (bloque de 90')
| Tiempo | Momento | Qué hace la profesora |
|---|---|---|
| 0–10' | **Inicio** | Proyecta *El sueño del caballero* (p. 64) y pregunta: ¿qué quiso comunicar el pintor? Presenta la pregunta del día: ¿necesitamos hoy un "desengaño del mundo"? Explica los controles. |
| 10–70' | **Juego** | Estudiantes juegan (individual o en parejas). La profesora circula; el juego avisa si alguien queda atrás. |
| 70–90' | **Cierre** | Diálogo grupal (pp. 110–111): ¿de qué podríamos desengañarnos hoy? (redes, filtros, la vida "perfecta"). Se comparan los finales elegidos. Revisión rápida de resultados por concepto. **Tarea:** ensayo de 5 párrafos de la p. 106 (puede usar *The Truman Show* como detonante, como propone el libro). |

**Bloque de 45':** inicio 3', juego 37–40', cierre 5' (una pregunta y la tarea).

## 10. Arte y audio
- **Greybox primero**, generado por código; el arte final reemplaza prefabs sin tocar lógica.
- Niebla: fog exponencial + postproceso (grano, viñeta, desaturación). Dorado: material metálico ocre, velas, bloom moderado.
- Galería: recreaciones 3D originales de las composiciones vanitas de Pereda y Murillo (las obras son del s. XVII).
- Audio original o CC0: zumbido grave, viento, campana, estática **procedural**; TTS para poemas como placeholder.
- Rendimiento: 60 fps a 1080p en gráfica integrada.

## 11. Fuentes
- Texto del Estudiante Lengua y Literatura 2° medio, Santillana (PDF oficial): https://www.curriculumnacional.cl/estudiante/621/articles-145685_recurso_pdf.pdf — Unidad 2, pp. 64–119.
- Programa, Unidad 3 "Lo divino y lo humano": https://www.curriculumnacional.cl/portal/Educacion-General/Lenguaje-y-comunicacion-Lengua-y-literatura/Lengua-y-literatura-2-Medio/79934:Unidad-3-Lo-divino-y-lo-humano-genero-lirico
