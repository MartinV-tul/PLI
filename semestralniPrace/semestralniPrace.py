import math

class NgramModel:
        
            nLessOneGrams = {}
            ngrams = {}
            vocabulary = set()
            precursorCount = {}
            separator = "|"
            length = 0
            size = 0
            def __init__(self, vocab, chars, size):#vytvoreni ngramoveho modelu
                self.nLessOneGrams = {}
                self.ngrams = {}
                self.vocabulary = set()
                self.precursorCount = {}
                self.size = size

                for v in vocab:
                    self.vocabulary.add(v)#kazdy znak je pridan do seznamu znaku, jelikoz je seznam znaku set nejsou v nem duplicity

                if (size == 0):#pokud je zadana nulova velikost ngramu dojde k navratu
                     return
                self.length = len(chars)#pocet vsech znaku
                for i in range(0, self.length):
                    if (size == 1):#pro pripad ze je vytvaren unigram
                        ngram = chars[i]
                        if not ngram in self.ngrams:
                            self.ngrams[ngram] = 0
                        self.ngrams[ngram] += 1
                        continue
                    if (i > size - 2):
    
                        index = i - size + 1
                        nLessOneGram = chars[index]
                        index += 1
                        for j in range(index, i):
                            nLessOneGram = chars[j] + self.separator + nLessOneGram;#nejprve je vytvoren n-1gram
                        
                        if not nLessOneGram in self.nLessOneGrams:#pokud neni n-1gram ve slovniku je do nej ulozen a je mu prirazena hodnota 0
                            self.nLessOneGrams[nLessOneGram] = 0
                        self.nLessOneGrams[nLessOneGram] += 1#zvyseni poctu n-1gramu ve slovniku
                        if i < self.length - 1:
                            ngram = chars[i] + self.separator + nLessOneGram#vytvoreni ngramu
                            if not ngram in self.ngrams:#pokud neni ngram ve slovniku je do nej ulozen a je mu prirazena hodnota 0
                                if not nLessOneGram in self.precursorCount:#pokud neni n-1gram ve slovniku predchudcu je do nej ulozen a je mu prirazena hodnota 0
                                    self.precursorCount[nLessOneGram] = 0
                                
                                self.precursorCount[nLessOneGram]+=1#zvyseni poctu pro kolik unikatnich ngramu je n-1gram predchudcem
                                self.ngrams[ngram] = 0
                            self.ngrams[ngram] += 1#zvyseni poctu ngramu ve slovniku

            def GetTextProbability(self,text):#vypocet pravdepodobnosti textu za predpokladu ze byl vygenerovan danym ngramovym modelem
                probability = 0
                length = len(text)
                for i in range(self.size - 1, length):
                    ngram = text[i]
                    for j in range(1, self.size):
                        ngram += "|" + text[i - j]
                    probability += math.log(self.GetWittenBellProbability(ngram), 2)#nejprve je ziskana pravdepodobnost ngramu vyhlazena metodou WittenBell a pote je z ni spocten logaritmus o zakladu 2
                return probability

            def GetWittenBellProbability(self,ngram):#vypocet pravdepodobnosti vyhlazene metodou WittenBell
                words = ngram.split(self.separator, 1)
                if len(words)== 1:#pokud se jedna o unigram
                    if not ngram in self.ngrams: 
                        return 1 / self.length#pokud dany unigram neni ve slovniku je jeho pravdepodobnost vypoctena jako by se v korpusu nachazel jednou
                    return self.ngrams[ngram] / self.length

                if not words[1] in self.precursorCount:#pokud neni predchudce ngramu ve slovniku bude pravdepodobnost vypoctena jako by byl ve slovniku jednou
                    T = 1
                    N = 1
                else:
                    T = self.precursorCount[words[1]]
                    N = self.nLessOneGrams[words[1]]
                Z = len(self.vocabulary) - T

                if not ngram in self.ngrams:#vypocet pravdepodobnosti
                    return T / (Z * N + T)
                return self.ngrams[ngram] / (N + T)


withoutDiacritic = False
skText = False

if (withoutDiacritic):
    korpusCZ = "KORPUS-CZ-ND.txt"
    korpusSK = "KORPUS-SK-2.txt"
    vocabulary = "abeceda-ND.txt"
else:
    korpusCZ = "KORPUS-CZ.txt"
    korpusSK = "KORPUS-SK.txt"
    vocabulary = "abeceda.txt"

fileNames = []

for i in range(0,7):
    fileName = "text" + str(i + 1)
    if (skText):
        fileName += "-SK"
    else:
        fileName += "-CZ"    
    if (withoutDiacritic):
        fileName += "-ND"
    fileName += ".txt"
    fileNames.append(fileName)

f = open("texty/" + vocabulary, "r", encoding="utf8")
vocabulary = f.read()
vocabulary = list(vocabulary)

f = open("texty/" + korpusCZ, "r", encoding="utf8")
text = f.read()
text.replace("\n", " ")
text = list(text)

ngramModelCZ = NgramModel(vocabulary,text,4)

f = open("texty/" + korpusSK, "r", encoding="utf8")
text = f.read()
text.replace("\n", " ")
ngramModelSK = NgramModel(vocabulary,text,4)

for fileName in fileNames:

    f = open("texty/" + fileName, "r", encoding="utf8")
    text = f.read()
    text.replace("\n", " ")
    text = list(text)

    probabilityCZ = ngramModelCZ.GetTextProbability(text)
    probabilitySK = ngramModelSK.GetTextProbability(text)

    if probabilityCZ > probabilitySK:
        print("Je větší pravděpodobnost, že text " + fileName + " je český(log. pravděpodobnost: " + str(probabilityCZ) + ") než slovenský(log. pravděpodobnost: " + str(probabilitySK) + ").")
    else:
        print("Je větší pravděpodobnost, že text " + fileName + " je slovenský(log. pravděpodobnost: " + str(probabilitySK) + ") než český(log. pravděpodobnost: " + str(probabilityCZ) + ").")

