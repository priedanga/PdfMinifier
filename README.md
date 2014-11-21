PdfMinifier
===========

Sumažina PDF dokumentuose esančius paveikslėlius. Galima užduoti iki kokios DPI raiškos sumažinti paveikslėlius, bei didesnį suspaudimą. Programa taipogi gali pabandyti sumažinti spalvų skaičių iki 256 ir išsaugoti paveikslėlį kaip GIF atvaizdą, taip sutaupant dar kažkiek kilobaitų.

## Žinomos problemos
* Jei įstatas į PDF dokumentą atvaizdas turi permatomumo savybę (permatomas fonas) gali būti atvaizduojamas juodas fonas.

## Bibliotekos
Darbui su PDF šį programa naudoja [iTextSharp biblioteką](http://sourceforge.net/projects/itextsharp/)

Geresnis JPEG atvaizdų suspadimas [JpegEncoder](http://www.codeproject.com/Articles/83225/A-Simple-JPEG-Encoder-in-C) © Arpan Jati

Geresnis GIF suspaudimas: [SimplePaletteQuantizer](http://www.codeproject.com/Articles/66341/A-Simple-Yet-Quite-Powerful-Palette-Quantizer-in-C) © Smart K8
