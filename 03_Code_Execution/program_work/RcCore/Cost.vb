Option Strict On
Imports System

''' <summary>ÇŞİDB 2026 birim fiyatlari (TL). Kaynak ve sayfa: 01_Literature/PRICES_2026.md (sayfa goruntusunden dogrulandi).
''' Tum degerler girdidir; kod icinde baska fiyat YOKTUR. Fiyat degisikligi yalniz bu sinifta ve PRICES_2026.md'de yapilir.</summary>
Public Class PriceSet
    ' Yerinde beton, pompa+nakil+dokum dahil (15.150.xxxx), TL/m3. Karar: kullanici 2026-10-28.
    Public ConcreteC25 As Double = 3524.35   ' 15.150.1005 C 25/30
    Public ConcreteC30 As Double = 3649.35   ' 15.150.1006 C 30/37
    Public ConcreteC35 As Double = 3836.85   ' 15.150.1007 C 35/45
    Public ConcreteC40 As Double = 3961.85   ' 15.150.1008 C 40/50
    Public RebarMaterial As Double = 24.2    ' 10.130.1704 ve 1705 (TL/kg), Ø8-12 ve Ø14-32 ayni fiyat
    Public RebarLabourSmall As Double = 46482.43  ' 15.160.1003 Ø8-Ø12 (TL/ton)
    Public RebarLabourLarge As Double = 44681.18  ' 15.160.1004 Ø14-Ø28 (TL/ton)
    Public Formwork As Double = 873.03       ' 15.180.1002 ahsap duz yuzeyli kalip (TL/m2)
    Public Fibre As Double = 90.0            ' 10.300.4014 tutkalli 0.90x60 mm (TL/kg)
    Public UsdRate As Double = 42.8843       ' TCMB 31.12.2025 USD Doviz Alis 42.8457 ve Satis 42.9229 aritmetik ortalamasi

    Public Function ConcretePrice(fc As Double) As Double
        If fc <= 25.0 Then Return ConcreteC25
        If fc <= 30.0 Then Return ConcreteC30
        If fc <= 35.0 Then Return ConcreteC35
        Return ConcreteC40
    End Function

    ''' <summary>Donati TL/kg (malzeme + isci); capa gore isci fiyati: Ø&lt;=12 kucuk, Ø&gt;12 buyuk (Ø&gt;28 icin 15.160.1005 gerekirse eklenecek).</summary>
    Public Function RebarPerKg(diameterMm As Double) As Double
        Dim labour As Double = If(diameterMm <= 12.0, RebarLabourSmall, RebarLabourLarge)
        Return RebarMaterial + labour / 1000.0
    End Function

    ''' <summary>Lif fiyati duyarlilik senaryosu: carpan uygulanmis kopya.</summary>
    Public Function WithFibreFactor(factor As Double) As PriceSet
        Dim p As PriceSet = DirectCast(Me.MemberwiseClone(), PriceSet)
        p.Fibre = Fibre * factor
        Return p
    End Function
End Class

''' <summary>Eleman/yapi maliyeti. Birimler: m3, kg, m2; sonuc TL. Kalem kalem dondurulur (Excel formul tablosu icin).</summary>
Public Structure CostBreakdown
    Public Concrete As Double
    Public Rebar As Double
    Public Formwork As Double
    Public Fibre As Double
    Public ReadOnly Property Total As Double
        Get
            Return Concrete + Rebar + Formwork + Fibre
        End Get
    End Property
End Structure

Public Module Cost
    ''' <summary>volumeM3: beton hacmi; rebarKg: bu capta donati kutlesi; fibreKgPerM3: lif dozaji (0 = lifsiz), fibreVolumeM3: lifli beton hacmi.</summary>
    Public Function Member(p As PriceSet, fc As Double, volumeM3 As Double, formworkM2 As Double,
                           rebarKgSmall As Double, rebarKgLarge As Double,
                           fibreKgPerM3 As Double, fibreVolumeM3 As Double) As CostBreakdown
        Dim c As CostBreakdown
        c.Concrete = volumeM3 * p.ConcretePrice(fc)
        c.Rebar = rebarKgSmall * p.RebarPerKg(12.0) + rebarKgLarge * p.RebarPerKg(16.0)
        c.Formwork = formworkM2 * p.Formwork
        c.Fibre = fibreKgPerM3 * fibreVolumeM3 * p.Fibre
        Return c
    End Function

    Public Function ToUsd(tl As Double, p As PriceSet) As Double
        Return tl / p.UsdRate
    End Function

    ''' <summary>Goreli maliyet (lifsiz = 100)</summary>
    Public Function Relative(total As Double, reference As Double) As Double
        Return 100.0 * total / reference
    End Function
End Module
