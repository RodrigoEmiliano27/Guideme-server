﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GuideMeServerMVC.Models
{
    [Table("tbTags")]
    public class TagViewModel:BaseViewModel
    {
        
        [Column("TAG")]
        public string TagId { get; set; }
        [Column("ID_ESTABELECIMENTO")]
        public int EstabelecimentoId { get; set; }
        [Column("ID_TIPO_TAG")]
        public int tipoTag { get; set; }
        [Column("NOME")]
        public string Nome { get; set; }

        [NotMapped]
        public List<TagsPaiViewModel>? TagsPai { get; set; }
    }
}
