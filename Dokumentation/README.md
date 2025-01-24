# Dokumentation
Her findes alt dokumentation til dette projekt.

# Casebeskrivelse
- Hotel koncern ønsker et booking system til alle koncernens hoteller.
Hvert hotel skal have sin egen side, hvor en kunde kan booke værelser.

- På hjemmesiden skal man kunne se eksempler på hvordan værelset ser ud.\
Kunden skal have mulighed for at kunne logge ind på hjemmesiden, hvor de kan vælge hoteller og booke værelser.\
De skal også have en side hvor de kan se sine bookings

- Der skal også være en side hvor kunder kan kontakte supporten, til f.eks. at anullere en booking.

- Systemet skal indeholde et login system hvor både ansatte og kunder skal kunde logge ind.\
Ansatte skal kunne tilknyttes til flere hoteller og have roller til at håndtere rettigheder for det hotel.

- De skal have en administrations side til selve koncernen for at kunne oprette og håndtere deres hoteller,\
tilføje ansattes brugere og styre deres roller.

- På hotellernes sider skal hotel administratoren kunde håndtere hotellets værelser og ansatte.

- Yderligere skal de have et ticket system til de ansatte, til at supportere kunder.\
***(Denne service skal køre separat som sin egen applikation.)***

## Roller:
- **Global Administrator:** Skal kunne oprette hoteller og brugere, og skal kunne håndtere hotellers ansatte og deres roller.
- **Hotel Administrator:** Skal kunne oprette oprette og håndtere de tilknyttede hotellers værelser.
- **Hotel Service:** Skal kunne se, rette og anullere bookinger af værelser på de tilknyttede hoteller.

# Kravspec

## Kort beskrivelse af produkt
Produktet er en hjemmeside der skal bruges til at booke hotelværelser hos forskellige hoteller. Man skal også kunne styre hotel arbejdere, baseret på hvad ens egent position hos hotellet er.

## Intro
Som en gæst/kunde hos et hotel skal man kunne vælge et hotel og så kunne booke et værelse hos det bestemte hotel. Derfra skal vores kode kunne registrere at værelset er booket hos hotellet. Man skal også have muligheden som Global Administrator for at kunne oprette Hoteller og derved Hotel Administratorer, som så har muligheden for at oprette Medarbejdere som har muligheden for at kunne booke et værelse til en kunde. Som hotel administrator skal man også kunne se tilstande af værelser (altså om de er booked eller ej)

## Funktionalitet
### Hovedside
Man skal kunne søge efter hoteller i forskellige dele af verdenen, og derfra kunne tilgå de forskellige hotellers side. Man skal også kunne have en mulighed for at logge ind fra forsiden.
Krav:
- Kunne logge ind
- Søge efter hotel et specifikt sted
- Hvis man logger ind som ansat (baseret på rolle) får man en administrator menu hvor man kan vælge hvilket hotel at administerer.
- Global administrator skal kunne tilføje og redigere hoteller og hotel administratore.

### Hotelside
Man skal have mulighed for at booke et værelse hos det valgte hotel. Man skal også have mulighed for at oprette en ticket hos det valgte hotel, hvilket hotellets medarbejdere skal kunne tilgå og hjælpe med.
Krav:
- Se alle tilgængelige af hottelets værelse som kunde.
- Se alle hotellets værelser som medarbejder, og kunne administeret dem. Alåts, kunne oprette eller slette bookings for kunder.
- Hotel administrator skal kunne tilføje & redigere værelser og medarbejdere.
- Som kunde skal man kunne oprette tickets.

### Administrator side
Tilpasset til hotel ansatte, hvor de skal kunne tilgå ticket system.
Krav:
- Man skal kunne se tickets hos det hotel man er ansat, og besvare dem.
